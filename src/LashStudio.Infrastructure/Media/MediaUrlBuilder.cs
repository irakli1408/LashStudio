using LashStudio.Application.Common.Abstractions;
using Microsoft.Extensions.Options;

namespace LashStudio.Infrastructure.Media
{
    public sealed class MediaUrlBuilder : IMediaUrlBuilder
    {
        private readonly MediaUrlOptions _opt;
        public MediaUrlBuilder(IOptions<MediaUrlOptions> opt) => _opt = opt.Value;

        public string Url(long assetId)
        {
            // Пример: https://cdn/asset/123  (или собери по PathFormat)
            if (string.IsNullOrWhiteSpace(_opt.BaseUrl))
                throw new InvalidOperationException("MediaOptions.BaseUrl is not configured");
            return $"{_opt.BaseUrl.TrimEnd('/')}/asset/{assetId}";
        }
    }
}
