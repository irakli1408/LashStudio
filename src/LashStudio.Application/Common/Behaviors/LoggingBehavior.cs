using MediatR;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace LashStudio.Application.Common.Behaviors;

public sealed class LoggingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly ILogger<LoggingBehavior<TReq, TRes>> _log;
    public LoggingBehavior(ILogger<LoggingBehavior<TReq, TRes>> log) => _log = log;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            return await next();
        }
        finally
        {
            sw.Stop();
            _log.LogInformation("Handled {Request} in {Elapsed} ms", typeof(TReq).Name, sw.ElapsedMilliseconds);
        }
    }
}
