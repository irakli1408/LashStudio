using LashStudio.Application.Common.Abstractions;
using Microsoft.Extensions.Caching.Memory;

namespace LashStudio.Infrastructure.Cache;

public sealed class MemoryAppCache : IAppCache
{
    private readonly IMemoryCache _mem;
    public MemoryAppCache(IMemoryCache mem) => _mem = mem;

    public bool TryGet<T>(string key, out T? value)
    {
        if (_mem.TryGetValue(key, out var obj) && obj is T t) { value = t; return true; }
        value = default; return false;
    }

    public void Set<T>(string key, T value, TimeSpan ttl) => _mem.Set(key, value!, ttl);
}
