using System.Globalization;
using LashStudio.Api.Config;
using Microsoft.Extensions.Options;

namespace LashStudio.Api.Middleware;

public sealed class CultureMiddleware
{
    private readonly RequestDelegate _next;
    private readonly LocalizationOptions _opt;

    public CultureMiddleware(RequestDelegate next, IOptions<LocalizationOptions> opt)
    { _next = next; _opt = opt.Value; }

    public async Task Invoke(HttpContext ctx)
    {
        // Ждём путь: /api/v1/{culture}/...
        var segs = ctx.Request.Path.Value?
            .Split('/', StringSplitOptions.RemoveEmptyEntries) ?? Array.Empty<string>();

        if (segs.Length < 3)
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = "missing_culture" });
            return;
        }

        var culture = segs[2].ToLowerInvariant();

        if (!_opt.SupportedCultures.Contains(culture, StringComparer.OrdinalIgnoreCase))
        {
            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = "unsupported_culture", culture });
            return;
        }

        var ci = new CultureInfo(culture);
        CultureInfo.CurrentCulture = ci;
        CultureInfo.CurrentUICulture = ci;
        ctx.Items["culture"] = culture;

        await _next(ctx);
    }
}
