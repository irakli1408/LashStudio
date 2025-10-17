using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Blog;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Queries.Posts.GetList
{
    public sealed class GetAdminPostListHandler
         : IRequestHandler<GetAdminPostListQuery, PagedResult<AdminPostListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly MediaOptions _media;

        public GetAdminPostListHandler(IAppDbContext db, IOptions<MediaOptions> media)
        {
            _db = db;
            _media = media.Value;
        }

        public async Task<PagedResult<AdminPostListItemVm>> Handle(GetAdminPostListQuery q, CancellationToken ct)
        {
            var page = q.Page <= 0 ? 1 : q.Page;
            var size = q.PageSize is <= 0 or > 200 ? 20 : q.PageSize;

            var baseQ = _db.Posts.AsNoTracking();

            if (q.IsPublished is not null)
                baseQ = baseQ.Where(p => (p.Status == PostStatus.Published) == q.IsPublished);

            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                var s = q.Search.Trim();
                baseQ = baseQ.Where(p => p.Locales.Any(l =>
                    l.Culture == q.Culture &&
                    (EF.Functions.Like(l.Title, $"%{s}%") || EF.Functions.Like(l.Slug, $"%{s}%"))));
            }

            baseQ = (q.Sort?.ToLowerInvariant()) switch
            {
                "createdat_asc" => baseQ.OrderBy(p => p.CreatedAt).ThenBy(p => p.Id),
                "publishedat_asc" => baseQ.OrderBy(p => p.PublishedAt ?? DateTime.MinValue).ThenBy(p => p.Id),
                "publishedat_desc" => baseQ.OrderByDescending(p => p.PublishedAt ?? DateTime.MinValue).ThenByDescending(p => p.Id),
                "title_asc" => baseQ.OrderBy(p => p.Locales.Where(l => l.Culture == q.Culture).Select(l => l.Title).FirstOrDefault()!)
                                           .ThenByDescending(p => p.CreatedAt),
                "title_desc" => baseQ.OrderByDescending(p => p.Locales.Where(l => l.Culture == q.Culture).Select(l => l.Title).FirstOrDefault()!)
                                           .ThenByDescending(p => p.CreatedAt),
                _ => baseQ.OrderByDescending(p => p.CreatedAt).ThenByDescending(p => p.Id),
            };

            var total = await baseQ.CountAsync(ct);

            var rows = await baseQ
                .Skip((page - 1) * size)
                .Take(size)
                .Select(p => new
                {
                    p.Id,
                    Status = p.Status.ToString(),
                    p.CreatedAt,
                    p.PublishedAt,
                    p.CoverMediaId,
                    CoverStoredPath = p.CoverMedia != null ? p.CoverMedia.StoredPath : null,
                    // выбор локали строго по нужной культуре; для Slug дадим фолбэк на SlugDefault
                    Title = p.Locales.Where(l => l.Culture == q.Culture).Select(l => l.Title).FirstOrDefault() ?? "",
                    Slug = p.Locales.Where(l => l.Culture == q.Culture).Select(l => l.Slug).FirstOrDefault() ?? p.SlugDefault
                })
                .ToListAsync(ct);

            var items = rows.Select(x => new AdminPostListItemVm(
                x.Id,
                x.Status,
                x.CreatedAt,
                x.PublishedAt,
                x.CoverMediaId,
                x.CoverStoredPath is null ? null
                    : $"{_media.RequestPath}/{x.CoverStoredPath}".Replace("//", "/").Replace("\\", "/"),
                x.Title,
                x.Slug
            )).ToList();

            return new PagedResult<AdminPostListItemVm>(
                Total: total,
                Page: page,
                PageSize: size,
                Items: items
            );
        }
    }
}
