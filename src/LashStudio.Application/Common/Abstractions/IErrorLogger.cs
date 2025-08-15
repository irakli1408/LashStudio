namespace LashStudio.Application.Common.Abstractions;

public interface IErrorLogger
{
    Task LogAsync(string message, string? stack, string? path, string? method, string? traceId, CancellationToken ct = default);
}
