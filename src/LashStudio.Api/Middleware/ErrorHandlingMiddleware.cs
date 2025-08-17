using FluentValidation;
using LashStudio.Application.Common.Abstractions;
using System.Net;
using System.Text.Json;

namespace LashStudio.Api.Middleware;

public sealed class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ErrorHandlingMiddleware(RequestDelegate next)
        => _next = next;

    // IErrorLogger берём как параметр Invoke — это per-request (scoped) резолв.
    public async Task Invoke(HttpContext ctx, IErrorLogger err)
    {
        try
        {
            await _next(ctx);
        }
        catch (ValidationException vex)
        {
            var details = vex.Errors.Select(e => new { e.PropertyName, e.ErrorMessage });

            await err.LogAsync(
                message: "Validation failed: " + JsonSerializer.Serialize(details),
                stack: null,
                path: ctx.Request.Path,
                method: ctx.Request.Method,
                traceId: ctx.TraceIdentifier,
                ct: ctx.RequestAborted);

            ctx.Response.StatusCode = StatusCodes.Status400BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = "validation_error", traceId = ctx.TraceIdentifier, details });
        }
        catch (KeyNotFoundException)
        {
            await err.LogAsync(
                message: "Not found",
                stack: null,
                path: ctx.Request.Path,
                method: ctx.Request.Method,
                traceId: ctx.TraceIdentifier,
                ct: ctx.RequestAborted);

            ctx.Response.StatusCode = StatusCodes.Status404NotFound;
            await ctx.Response.WriteAsJsonAsync(new { error = "not_found", traceId = ctx.TraceIdentifier });
        }
        catch (ArgumentException aex)
        {
            ctx.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            await ctx.Response.WriteAsJsonAsync(new { error = aex.Message, traceId = ctx.TraceIdentifier });
        }
        catch (Exception ex)
        {
            await err.LogAsync(
                message: ex.Message,
                stack: ex.StackTrace,
                path: ctx.Request.Path,
                method: ctx.Request.Method,
                traceId: ctx.TraceIdentifier,
                ct: ctx.RequestAborted);

            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await ctx.Response.WriteAsJsonAsync(new { error = "server_error", traceId = ctx.TraceIdentifier });
        }       
    }
}
