using LashStudio.Api.Config;
using LashStudio.Api.Localization;
using LashStudio.Api.Middleware;
using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Behaviors;
using LashStudio.Infrastructure.Cache;
using LashStudio.Infrastructure.Logs;
using LashStudio.Infrastructure.Persistence;
using LashStudio.Infrastructure.Time;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(
        builder.Configuration.GetConnectionString("LashStudioDB")
        ?? "Server=DESKTOP-BT4EB20;Database=LashStudio;Trusted_Connection=True;TrustServerCertificate=True"));

builder.Services.AddScoped<IAppDbContext>(sp => sp.GetRequiredService<AppDbContext>());
builder.Services.AddMemoryCache();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
}

builder.Services.AddScoped<IErrorLogger, ErrorLogger>();


builder.Services.Configure<LashStudio.Api.Config.LocalizationOptions>(builder.Configuration.GetSection("Localization"));

builder.Services.AddLocalization();

builder.Services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();

// behaviors
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));

// abstractions
builder.Services.AddSingleton<IAppCache, MemoryAppCache>();
builder.Services.AddSingleton<IDateTime, SystemDateTime>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.MapGet("/health/live", () => Results.Ok("OK"));


app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseMiddleware<CultureMiddleware>();

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

var v1 = app.MapGroup("/api/v1/{culture}");
v1.MapGet("/ping", (string culture) => Results.Ok(new { culture }));

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
