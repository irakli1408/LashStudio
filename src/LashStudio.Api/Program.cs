using Asp.Versioning;
using LashStudio.Api.Localization;
using LashStudio.Api.Middleware;
using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Handlers.Admin.Commands; // для сканирования MediatR
using LashStudio.Application.Handlers.Admin.Commands.Media;
using LashStudio.Infrastructure.Cache;
using LashStudio.Infrastructure.Logs;
using LashStudio.Infrastructure.Persistence;
using LashStudio.Infrastructure.Storage;
using LashStudio.Infrastructure.Time;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Db
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("LashStudioDB")
        ?? "Server=DESKTOP-BT4EB20;Database=LashStudio;Trusted_Connection=True;TrustServerCertificate=True"));
builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(LashStudio.Application.Handlers.Admin.Commands.Publish.Post.PublishPostCommand).Assembly));


// MediatR (сканим Application)
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(UploadMediaHandler).Assembly));

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

app.MapControllers();

app.Run();
