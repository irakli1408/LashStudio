namespace LashStudio.Application.Common.Abstractions
{
    public interface IMediaUrlBuilder
    {
        string Url(long assetId);                      // полный URL
        string? TryUrl(long? assetId)                  // удобный хелпер
            => assetId is null ? null : Url(assetId.Value);
    }
}
