using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Helpers;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminList
{
    public sealed class GetCourseAdminListHandler
        : IRequestHandler<GetCourseAdminListQuery, PagedResult<CourseAdminListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;

        public GetCourseAdminListHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        {
            _db = db;
            _opt = opt;
        }

        public async Task<PagedResult<CourseAdminListItemVm>> Handle(GetCourseAdminListQuery q, CancellationToken ct)
        {
            var baseQ = _db.Courses.AsNoTracking();

            string? s = null;
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                s = q.Search.Trim().ToLower();
                baseQ = baseQ.Where(c =>
                    c.Slug.ToLower().Contains(s) ||
                    c.Locales.Any(l => (l.Title ?? "").ToLower().Contains(s)));
            }

            var total = await baseQ.CountAsync(ct);

            // 1) Берём страницу курсов
            var rows = await baseQ
                .OrderByDescending(x => x.PublishedAtUtc ?? x.CreatedAtUtc)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Slug,
                    c.Level,
                    c.IsActive,
                    c.CreatedAtUtc,
                    c.PublishedAtUtc,
                    TitleAny = (s != null
                            ? c.Locales
                                .Where(l => ((l.Title ?? "").ToLower().Contains(s)))
                                .Select(l => l.Title)
                                .FirstOrDefault()
                            : null)
                        ?? c.Locales.Select(l => l.Title).FirstOrDefault(),
                    c.CoverMediaId
                })
                .ToListAsync(ct);

            // 2) OwnerKey'и для батча
            var ownerKeys = rows
                .Select(r => r.Id.ToString(CultureInfo.InvariantCulture))
                .ToArray();

            // 3) Одним запросом тянем все медиа для этой страницы
            var attachments = await _db.MediaAttachments.AsNoTracking()
                .Where(a => a.OwnerType == MediaOwnerType.Course && ownerKeys.Contains(a.OwnerKey))
                .Select(a => new
                {
                    a.OwnerKey,
                    a.MediaAssetId,
                    a.SortOrder,
                    a.IsCover,
                    a.CreatedAtUtc,
                    a.MediaAsset.StoredPath,
                    a.MediaAsset.ThumbStoredPath,
                    a.MediaAsset.Type
                })
                .ToListAsync(ct);

            // 4) Группируем по владельцу и строим VM (Url собираем в памяти)
            var mediaByOwner = attachments
                .GroupBy(a => a.OwnerKey)
                .ToDictionary(
                    g => g.Key,
                    g => g
                        .OrderBy(x => x.SortOrder)
                        .Select(x => new CourseMediaVm(
                            AssetId: x.MediaAssetId,
                            Url: MediaUrlHelper.ToUrl(_opt.Value, x.StoredPath),
                            ThumbUrl: x.ThumbStoredPath is null
                                ? null
                                : MediaUrlHelper.ToUrl(_opt.Value, x.ThumbStoredPath),
                            MediaType: x.Type,
                            SortOrder: x.SortOrder,
                            IsCover: x.IsCover,
                            CreatedAtUtc: x.CreatedAtUtc
                        ))
                        .ToList()
                );

            // 5) Собираем итог
            var items = rows.Select(r =>
            {
                var key = r.Id.ToString(CultureInfo.InvariantCulture);
                mediaByOwner.TryGetValue(key, out var media);
                media ??= new List<CourseMediaVm>();


                var coverMediaId = r.CoverMediaId
                    ?? media.FirstOrDefault(m => m.IsCover)?.AssetId;

                return new CourseAdminListItemVm(
                    Id: r.Id,
                    Slug: r.Slug,
                    Level: r.Level,
                    IsActive: r.IsActive,
                    CreatedAtUtc: r.CreatedAtUtc,
                    PublishedAtUtc: r.PublishedAtUtc,
                    TitleAny: r.TitleAny,
                    CoverMediaId: coverMediaId,
                    Media: media
                );
            }).ToList();

            return new PagedResult<CourseAdminListItemVm>(total, q.Page, q.PageSize, items);
        }
    }
}
