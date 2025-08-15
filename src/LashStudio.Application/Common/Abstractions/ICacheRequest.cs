namespace LashStudio.Application.Common.Abstractions;

public interface ICacheRequest
{
    string CacheKey { get; }
    TimeSpan Ttl { get; }
}
