using Asp.Versioning;
using LashStudio.Api.Localization;
using LashStudio.Api.Middleware;
using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Handlers.Admin.Commands.Media.Upload;
using LashStudio.Application.Handlers.Admin.Commands.Publish.Common.Helper;
using LashStudio.Domain.Auth;
using LashStudio.Infrastructure.Auth;
using LashStudio.Infrastructure.Cache;
using LashStudio.Infrastructure.Config.CurrentStateService;
using LashStudio.Infrastructure.Config.Media;
using LashStudio.Infrastructure.Initialization;
using LashStudio.Infrastructure.Logs;
using LashStudio.Infrastructure.Media;
using LashStudio.Infrastructure.Persistence;
using LashStudio.Infrastructure.Storage;
using LashStudio.Infrastructure.Time;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LashStudioDB")
        ?? "Server=DESKTOP-BT4EB20;Database=LashStudio;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddSingleton(TimeProvider.System);

builder.Services
    .AddIdentityCore<ApplicationUser>(o =>
    {
        o.Password.RequiredLength = 6;
        o.Password.RequireNonAlphanumeric = false;
        o.Lockout.MaxFailedAccessAttempts = 5;
        o.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
        o.SignIn.RequireConfirmedEmail = false; // можно включить позже
    })
    .AddRoles<IdentityRole<long>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddRoleManager<RoleManager<IdentityRole<long>>>();

builder.Services.AddAuthorization(); // политики добавим на шаге 2

builder.Services.AddScoped<IRefreshTokenStore, EfRefreshTokenStore>();
builder.Services.AddScoped<IJwtProvider, JwtTokenService>();


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Admin.Commands.Publish.Post.PublishPostCommand).Assembly));

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(SetActiveCourseHandler).Assembly  // Application
    );
});

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

//Media
builder.Services.Configure<MediaUrlOptions>(
    builder.Configuration.GetSection("Media"));

builder.Services.AddScoped<IMediaUrlBuilder, MediaUrlBuilder>();

// MediatR (сканим Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UploadMediaHandler).Assembly));


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "LashStudio API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new()
    {
        Description = "JWT. Введите только токен без слова Bearer",
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

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    // Понятный ответ при 429
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

    // Глобальный лимит (админка исключена)
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(ctx =>
    {
        var path = ctx.Request.Path.Value ?? string.Empty;

        // пропускаем админские роуты: /api/v{ver}/{culture}/admin/...
        if (path.Contains("/admin/", StringComparison.OrdinalIgnoreCase))
            return RateLimitPartition.GetNoLimiter("admin");

        var key = GetClientKey(ctx);
        return RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: key,
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 3,                    // глобально: 80 запросов
                Window = TimeSpan.FromMinutes(1),    // в минуту
                QueueLimit = 0,
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
    });

    // Точечная политика для "тяжёлых" публичных операций (напр. скачивания)
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

builder.Services.AddOutputCache(o =>
{
    // Базовая политика для публичных GET — 60 секунд
    o.AddPolicy("public-60s", b => b
        .Expire(TimeSpan.FromSeconds(60))
        // у тебя культура в маршруте: /api/v{version}/{culture}/...
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        // если есть пагинация/фильтры — добавь их:
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        // (опционально) префикс ключа, удобно для группировки
        .SetCacheKeyPrefix("pub"));

    // Длиннее с тегами (на будущее, если захочешь 10 минут + сброс из админки)
    o.AddPolicy("public-10m-tagged", b => b
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        .Tag("services")); // общий тег раздела услуг
});

// Identity
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
    .AddSignInManager();

// JWT
builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()!;

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

builder.Services.AddAuthorization(opt =>
{
    opt.AddPolicy("RequireAdmin", p => p.RequireRole("Admin", "SuperAdmin"));
    opt.AddPolicy("RequireSuperAdmin", p => p.RequireRole("SuperAdmin"));
});


// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Auth.Command.Register.RegisterHandler).Assembly));

// Абстракции
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IJwtProvider, JwtTokenService>();

// Кэш/время/логер
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IAppCache, MemoryAppCache>();
builder.Services.AddSingleton<IDateTime, SystemDateTime>();
builder.Services.AddScoped<IErrorLogger, ErrorLogger>();

// Localization (строки из БД)
builder.Services.AddLocalization();
builder.Services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();
builder.Services.Configure<LashStudio.Api.Config.LocalizationOptions>(builder.Configuration.GetSection("Localization"));

// Media
builder.Services.Configure<MediaOptions>(builder.Configuration.GetSection("Media"));
builder.Services.AddScoped<IFileStorage, LocalFileStorage>();
builder.Services.Configure<FormOptions>(o => o.MultipartBodyLengthLimit = 2L * 1024 * 1024 * 1024);
builder.Services.AddScoped<IMediaAttachmentService, MediaAttachmentService>();

//CurrentCulture
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentStateService, CurrentStateService>();

builder.Services.AddOutputCache(o =>
{
    // Базовая политика для публичных GET — 60 секунд
    o.AddPolicy("public-60s", b => b
        .Expire(TimeSpan.FromSeconds(60))
        // у тебя культура в маршруте: /api/v{version}/{culture}/...
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        // если есть пагинация/фильтры — добавь их:
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        // (опционально) префикс ключа, удобно для группировки
        .SetCacheKeyPrefix("pub"));

    // Длиннее с тегами (на будущее, если захочешь 10 минут + сброс из админки)
    o.AddPolicy("public-10m-tagged", b => b
        .Expire(TimeSpan.FromMinutes(10))
        .SetVaryByRouteValue("culture")
        .SetVaryByRouteValue("version")
        .SetVaryByQuery("page", "pageSize", "sort", "slug")
        .Tag("services")); // общий тег раздела услуг
});

// Controllers + versioning
builder.Services.AddControllers();
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
    o.ApiVersionReader = new UrlSegmentApiVersionReader();
});

// Swagger только в dev
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}


var app = builder.Build();

// Раздача /media
var mediaOpt = app.Services.GetRequiredService<IOptions<MediaOptions>>().Value;
var physical = Path.Combine(app.Environment.ContentRootPath, mediaOpt.RootPath);
Directory.CreateDirectory(physical);
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(physical),
    RequestPath = mediaOpt.RequestPath
});

// Глобальные middleware ДО MapControllers
app.UseMiddleware<ErrorHandlingMiddleware>();
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/api/v", StringComparison.OrdinalIgnoreCase),
    branch => branch.UseMiddleware<CultureMiddleware>());

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseForwardedHeaders(new ForwardedHeadersOptions
//{Локально/без прокси — можно не добавлять.

//За прокси — рекомендуется добавить (с KnownProxies/Networks), чтобы и rate limiter, и логи/редиректы/ссылки видели настоящий IP и схему https, и чтобы никто не обошёл лимиты подделкой заголовков.
//    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
//    // при необходимости заполни KnownProxies/KnownNetworks
//});

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

await DatabaseSeeder.SeedAsync(app.Services, app.Configuration, app.Environment);

app.UseRateLimiter();
app.UseOutputCache();
app.MapControllers();

app.Run();


static string GetClientKey(HttpContext ctx)
{
    var fwd = ctx.Request.Headers["X-Forwarded-For"].ToString();
    if (!string.IsNullOrWhiteSpace(fwd))
    {
        var first = fwd.Split(',')[0].Trim();
        if (!string.IsNullOrWhiteSpace(first))
            return first;
    }
    return ctx.Connection.RemoteIpAddress?.ToString() ?? "unknown";
}
