using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Media;
using LashStudio.Application.Contracts.Media;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Media.MediaAssets
{
    public sealed class GetMediaAssetsHandler
     : IRequestHandler<GetMediaAssetsQuery, PagedResult<MediaAssetListItemVm>>
    {
        private readonly IAppDbContext _db;
        public GetMediaAssetsHandler(IAppDbContext db) => _db = db;

        public async Task<PagedResult<MediaAssetListItemVm>> Handle(GetMediaAssetsQuery q, CancellationToken ct)
        {
            // Глобальный фильтр HasQueryFilter(!IsDeleted) уже применён
            var qry = _db.MediaAssets.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                qry = qry.Where(a => a.OriginalFileName.Contains(s) || a.StoredPath.Contains(s));
            }

            if (q.Type is not null)
                qry = qry.Where(a => a.Type == q.Type);

            if (!string.IsNullOrWhiteSpace(q.Extension))
                qry = qry.Where(a => a.Extension == q.Extension);

            if (q.FromUtc is not null)
                qry = qry.Where(a => a.CreatedAtUtc >= q.FromUtc);

            if (q.ToUtc is not null)
                qry = qry.Where(a => a.CreatedAtUtc < q.ToUtc);

            qry = q.OrderBy switch
            {
                MediaAssetOrderBy.CreatedAsc => qry.OrderBy(a => a.CreatedAtUtc),
                MediaAssetOrderBy.FileNameAsc => qry.OrderBy(a => a.OriginalFileName),
                MediaAssetOrderBy.FileNameDesc => qry.OrderByDescending(a => a.OriginalFileName),
                MediaAssetOrderBy.SizeAsc => qry.OrderBy(a => a.SizeBytes),
                MediaAssetOrderBy.SizeDesc => qry.OrderByDescending(a => a.SizeBytes),
                _ => qry.OrderByDescending(a => a.CreatedAtUtc),
            };

            var total = await qry.CountAsync(ct);

            var items = await qry
                .Skip(q.Skip)
                .Take(q.Take)
                .Select(a => new MediaAssetListItemVm(
                    a.Id,
                    a.Type,
                    a.OriginalFileName,
                    a.StoredPath,
                    a.ContentType,
                    a.SizeBytes,
                    a.CreatedAtUtc,
                    a.Extension,
                    a.IsDeleted,
                    a.DeletedAtUtc,
                    // считаем привязки без загрузки коллекции
                    _db.MediaAttachments.Count(ma => ma.MediaAssetId == a.Id)
                ))
                .ToListAsync(ct);

            return new PagedResult<MediaAssetListItemVm>(total,0,0, items);
        }
    }
}
