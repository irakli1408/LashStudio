using Asp.Versioning;
using LashStudio.Api.Localization;
using LashStudio.Api.Middleware;
using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Domain.Auth;
using LashStudio.Infrastructure.Auth;
using LashStudio.Infrastructure.Cache;
using LashStudio.Infrastructure.Config.CurrentStateService;
using LashStudio.Infrastructure.Config.Media;
using LashStudio.Infrastructure.Initialization;
using LashStudio.Infrastructure.Logs;
using LashStudio.Infrastructure.Media;
using LashStudio.Infrastructure.Persistence;
using LashStudio.Infrastructure.SmtpOption;
using LashStudio.Infrastructure.Storage;
using LashStudio.Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ------------------------------- Configuration --------------------------------
// bind smtp options (appsettings.json should have "Smtp": { ... })
builder.Services.Configure<SmtpOptions>(builder.Configuration.GetSection("Smtp"));

// ------------------------------- Infrastructure / App services -----------------
// Email sender: DEV implementation (logs). For production replace with SmtpEmailSender.
//builder.Services.AddScoped<IEmailSender, DevEmailSender>();
 builder.Services.AddScoped<IEmailSender, SmtpEmailSender>(); // <-- enable in PROD

// Token lifespan for Identity password reset tokens
builder.Services.Configure<DataProtectionTokenProviderOptions>(o =>
{
    o.TokenLifespan = TimeSpan.FromMinutes(45);
});

// DbContext + IAppDbContext
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LashStudioDB")
        ?? "Server=.;Database=LashStudio;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

// Time provider / cache / logger
builder.Services.AddSingleton(TimeProvider.System);
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAppCache, MemoryAppCache>();
builder.Services.AddSingleton<IDateTime, SystemDateTime>();
builder.Services.AddScoped<IErrorLogger, ErrorLogger>();



// CORS (dev-only) — разрешаем любые loopback-хосты (localhost, 127.0.0.1) на любых портах
builder.Services.AddCors(opt =>
{
    opt.AddPolicy("dev-cors", p =>
    {
        p.SetIsOriginAllowed(origin =>
        {
            // Разрешить все локальные origin-ы (http/https, любой порт)
            if (Uri.TryCreate(origin, UriKind.Absolute, out var uri))
                return uri.IsLoopback; // localhost / 127.0.0.1

            return false;
        })
        .AllowAnyHeader()
        .AllowAnyMethod()
        // если фронт шлёт куки/Authorization заголовок из браузера — оставь AllowCredentials
        .AllowCredentials()
        // полезно, чтобы фронт видел эти заголовки
        .WithExposedHeaders("X-RateLimit-Policy", "Retry-After", "Content-Disposition");
    });

    // (опционально) фиксированный список origin-ов, если не нужны "все локальные порты"
    opt.AddPolicy("dev-cors-fixed", p =>
    {
        p.WithOrigins(
            "http://localhost:3000",   // React
            "http://localhost:5173",   // Vite
            "http://localhost:4200",   // Angular
            "http://localhost:19006",  // Expo
            "http://127.0.0.1:5173",
            "https://localhost:3000",
            "https://localhost:5173"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials()
        .WithExposedHeaders("X-RateLimit-Policy", "Retry-After", "Content-Disposition");
    });
});





// Identity (one registration — ensure ApplicationUser is your Identity user)
builder.Services
    .AddIdentityCore<ApplicationUser>(o =>
    {
        o.Password.RequiredLength = 6;
        o.Password.RequireNonAlphanumeric = false;
        o.Lockout.MaxFailedAccessAttempts = 5;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        o.SignIn.RequireConfirmedEmail = false;
    })
    .AddRoles<IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddRoleManager<RoleManager<IdentityRole<long>>>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdmin", p => p.RequireRole("Admin", "SuperAdmin"));
    opt.AddPolicy("RequireSuperAdmin", p => p.RequireRole("SuperAdmin"));
});

// JWT (если используешь)
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;
if (jwt is not null)
{
    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(opt =>
        {
            opt.TokenValidationParameters = new()
            {
                ValidateIssuer = true,
                ValidIssuer = jwt.Issuer,
                ValidateAudience = true,
                ValidAudience = jwt.Audience,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30)
            };
        });
}

// App abstractions
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IJwtProvider, JwtTokenService>();
builder.Services.AddScoped<IRefreshTokenStore, EfRefreshTokenStore>();

// Media, storage and current state service
builder.Services.Configure<MediaOptions>(builder.Configuration.GetSection("Media"));
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024);
builder.Services.AddScoped<IMediaAttachmentService, MediaAttachmentService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentStateService, CurrentStateService>();

// Localization (DB-backed string localizer if you use it)
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
builder.Services.Configure<LashStudio.Api.Config.LocalizationOptions>(builder.Configuration.GetSection("Localization"));

// MediatR (register Application handlers — adapt assembly types if needed)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Auth.Command.Register.RegisterHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Admin.Commands.Publish.Common.Helper.SetActiveCourseHandler).Assembly));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Admin.Commands.Media.Upload.UploadMediaHandler).Assembly));

// Controllers / JSON
builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

// API Versioning
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Swagger only in dev
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new() { Title = "LashStudio API", Version = "v1" });
        c.AddSecurityDefinition("Bearer", new()
        {
            Description = "JWT. Enter token without 'Bearer' prefix",
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT"
        });
        c.AddSecurityRequirement(new()
        {
            {
                new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
                Array.Empty<string>()
            }
        });
    });
}

// Output cache (one registration)
builder.Services.AddOutputCache(o =>
{
    o.AddPolicy("public-60s", b => b
        .Expire(TimeSpan.FromSeconds(60))
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        .SetCacheKeyPrefix("pub"));

    o.AddPolicy("public-10m-tagged", b => b
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        .Tag("services"));
});

// ------------------------------- Rate limiter ---------------------------------
// Configure rate limiter options (global limiter + named policies)
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.OnRejected = async (ctx, token) =>
    {
        TimeSpan? retryAfter = null;
        if (ctx.Lease.TryGetMetadata(MetadataName.RetryAfter, out var ra))
            retryAfter = ra;

        if (retryAfter is { } t)
            ctx.HttpContext.Response.Headers["Retry-After"] = ((int)Math.Ceiling(t.TotalSeconds)).ToString();

        ctx.HttpContext.Response.Headers["X-RateLimit-Policy"] = "global-fixed-window; 80 req / 1m";
        ctx.HttpContext.Response.ContentType = "application/problem+json; charset=utf-8";

        var problem = new
        {
            type = "https://httpstatuses.com/429",
            title = "Too Many Requests",
            status = 429,
            detail = retryAfter is null
                ? "You have sent too many requests. Please try again later."
                : $"Rate limit exceeded. Retry after ~{Math.Ceiling(retryAfter.Value.TotalSeconds)} seconds.",
            traceId = ctx.HttpContext.TraceIdentifier
        };

        await ctx.HttpContext.Response.WriteAsJsonAsync(problem, cancellationToken: token);
    };

    // Global limiter (exclude admin routes)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var path = ctx.Request.Path.Value ?? string.Empty;
        if (path.Contains("/admin/", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetNoLimiter("admin");

        var key = GetClientKey(ctx);
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 80,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    // Named limiter for forgot-password endpoints
    options.AddFixedWindowLimiter("pwd-reset", o =>
    {
        o.Window = TimeSpan.FromMinutes(1);
        o.PermitLimit = 3;
        o.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        o.QueueLimit = 0;
    });

    // Additional named policy example
    options.AddPolicy("downloads-3-per-minute", ctx =>
    {
        var key = GetClientKey(ctx);
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 3,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            });
    });
});
builder.Services.Configure<MediaUrlOptions>(builder.Configuration.GetSection("Media"));
builder.Services.AddScoped<IMediaUrlBuilder, MediaUrlBuilder>();
builder.Services.AddHttpContextAccessor();

// ------------------------------- Build app ------------------------------------
var app = builder.Build();

// Ensure media folder exists and expose static files
var mediaOpt = app.Services.GetRequiredService<IOptions<MediaOptions>>().Value;
var physical = Path.Combine(app.Environment.ContentRootPath, mediaOpt.RootPath);
Directory.CreateDirectory(physical);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(physical),
    RequestPath = mediaOpt.RequestPath
});

// Middleware pipeline
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api/v", StringComparison.OrdinalIgnoreCase),
    branch => branch.UseMiddleware<CultureMiddleware>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseCors("dev-cors"); // или "dev-cors-fixed"
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Rate limiter & output cache middlewares
app.UseRateLimiter();
app.UseOutputCache();

await DatabaseSeeder.SeedAsync(app.Services, app.Configuration, app.Environment);

app.MapControllers();

app.Run();

// ------------------------------- Helpers --------------------------------------
static string GetClientKey(HttpContext ctx)
{
    // prefer X-Forwarded-For if behind proxy (Nginx, Cloud)
    var fwd = ctx.Request.Headers["X-Forwarded-For"].ToString();
    if (!string.IsNullOrWhiteSpace(fwd))
    {
        var first = fwd.Split(',')[0].Trim();
        if (!string.IsNullOrWhiteSpace(first))
            return first;
    }

    // fallback to direct remote IP
    return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
