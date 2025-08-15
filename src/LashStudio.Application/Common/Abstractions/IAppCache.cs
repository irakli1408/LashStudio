namespace LashStudio.Application.Common.Abstractions;

public interface IAppCache
{
    bool TryGet<T>(string key, out T? value);
    void Set<T>(string key, T value, TimeSpan ttl);
}
