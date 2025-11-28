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

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseList
{
    public sealed class GetCourseListHandler
        : IRequestHandler<GetCourseListQuery, PagedResult<CourseListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;

        public GetCourseListHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        {
            _db = db;
            _opt = opt;
        }

        public async Task<PagedResult<CourseListItemVm>> Handle(GetCourseListQuery q, CancellationToken ct)
        {
            var baseQ = _db.Courses.AsNoTracking()
                .Where(x => x.IsActive)
                .Where(x => !q.Level.HasValue || x.Level == q.Level.Value);

            var total = await baseQ.CountAsync(ct);

            // 1) берём страницу курсов (+ Id для ownerKey)
            var rows = await baseQ
                .OrderByDescending(x => x.PublishedAtUtc ?? x.CreatedAtUtc)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(x => new
                {
                    x.Id,
                    x.Slug,
                    x.Level,
                    x.Price,
                    x.DurationHours,
                    x.CoverMediaId,
                    Title = x.Locales
                        .Where(l => l.Culture == q.Culture)
                        .Select(l => l.Title)
                        .FirstOrDefault()
                        ?? x.Locales.Select(l => l.Title).FirstOrDefault()
                })
                .ToListAsync(ct);

            // 2) готовим ключи владельцев для батча
            var ownerKeys = rows
                .Select(r => r.Id.ToString(CultureInfo.InvariantCulture))
                .ToArray();

            // 3) одним запросом получаем все вложения для страницы
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

            // 4) группируем по ownerKey и строим VM со ссылками
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

            // 5) собираем итоговые элементы
            var items = rows.Select(r =>
            {
                var key = r.Id.ToString(CultureInfo.InvariantCulture);
                mediaByOwner.TryGetValue(key, out var mediaList);
                mediaList ??= new List<CourseMediaVm>();

                // cover: сначала по CoverMediaId, иначе — по IsCover
                var coverMedia = (r.CoverMediaId is not null
                        ? mediaList.FirstOrDefault(m => m.AssetId == r.CoverMediaId.Value)
                        : null)
                    ?? mediaList.FirstOrDefault(m => m.IsCover);

                var coverUrl = coverMedia?.Url;

                return new CourseListItemVm(
                    Slug: r.Slug,
                    Title: r.Title ?? "(no title)",
                    Level: r.Level,
                    Price: r.Price,
                    DurationHours: r.DurationHours,
                    CoverUrl: coverUrl,
                    Media: mediaList
                );
            }).ToList();

            return new PagedResult<CourseListItemVm>(total, q.Page, q.PageSize, items);
        }
    }
}
