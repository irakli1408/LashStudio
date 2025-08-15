using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Common.Behaviors;

public sealed class CachingBehavior<TReq, TRes> : IPipelineBehavior<TReq, TRes>
{
    private readonly IAppCache _cache;
    public CachingBehavior(IAppCache cache) => _cache = cache;

    public async Task<TRes> Handle(TReq request, RequestHandlerDelegate<TRes> next, CancellationToken ct)
    {
        if (request is not ICacheRequest c)
            return await next();

        if (_cache.TryGet<TRes>(c.CacheKey, out var cached) && cached is not null)
            return cached;

        var result = await next();
        _cache.Set(c.CacheKey, result!, c.Ttl);
        return result;
    }
}
