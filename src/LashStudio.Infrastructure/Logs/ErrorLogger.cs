using LashStudio.Application.Common.Abstractions;
using LashStudio.Infrastructure.Persistence;

namespace LashStudio.Infrastructure.Logs;

public sealed class ErrorLogger : IErrorLogger
{
    private readonly AppDbContext _db;
    public ErrorLogger(AppDbContext db) => _db = db;

    public async Task LogAsync(string message, string? stack, string? path, string? method, string? traceId, CancellationToken ct = default)
    {
        _db.Logs.Add(new LogEntry
        {
            CreatedAtUtc = DateTime.UtcNow,
            Level = "Error",
            Message = message,
            StackTrace = stack,
            Path = path,
            Method = method,
            TraceId = traceId
        });
        await _db.SaveChangesAsync(ct);
    }
}
