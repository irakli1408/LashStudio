using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Contracts.Media;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Queries.Media.GetMediaLibrary
{
    public sealed class GetMediaLibraryHandler : IRequestHandler<GetMediaLibraryQuery, MediaLibraryVm>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;
        private readonly IMediaUrlBuilder _urlBuilder;

        public GetMediaLibraryHandler(IAppDbContext db, IOptions<MediaOptions> opt, IMediaUrlBuilder urlBuilder)
            => (_db, _opt, _urlBuilder) = (db, opt, urlBuilder);

        public async Task<MediaLibraryVm> Handle(GetMediaLibraryQuery q, CancellationToken ct)
        {
            // 1) тянем из БД только то, что переводится в SQL
            var raw = await _db.MediaAssets
                .AsNoTracking()
                .Where(m => !m.IsDeleted)
                .OrderByDescending(m => m.Id)
                .Select(m => new
                {
                    m.Id,
                    m.OriginalFileName,
                    MediaType = m.Type == MediaType.Video ? "video" : "image",
                    m.StoredPath
                })
                .ToListAsync(ct);

            // 2) собираем публичный URL уже в памяти
            var items = raw.Select(r => new MediaItemVm(
                AssetId: r.Id,
                Name: r.OriginalFileName,
                MediaType: r.MediaType,                               // "image" | "video"
                Url: $"{_opt.Value.RequestPath.TrimEnd('/')}/{r.StoredPath}"
                        .Replace("//", "/").Replace("\\", "/")
            // Или если хочешь через билдер по Id:
            // Url: _urlBuilder.Url(r.Id)
            )).ToList();

            var photos = items.Where(x => x.MediaType == "image").ToList();
            var videos = items.Where(x => x.MediaType == "video").ToList();

            return new MediaLibraryVm(photos, videos);
        }
    }

}

