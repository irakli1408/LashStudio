using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs
{
    public sealed class GetBlogListHandler
    : IRequestHandler<GetBlogListQuery, PagedResult<BlogListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly MediaOptions _opt;
        public GetBlogListHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        { _db = db; _opt = opt.Value; }

        public async Task<PagedResult<BlogListItemVm>> Handle(GetBlogListQuery q, CancellationToken ct)
        {
            var culture = q.Culture.Trim().ToLowerInvariant();
            var page = Math.Max(1, q.Page);
            var size = Math.Clamp(q.PageSize, 1, 100);

            var baseQuery = _db.Posts
                .AsNoTracking()
                .Where(p => p.Status == Domain.Blog.PostStatus.Published)
                .Include(p => p.Locales)
                .Include(p => p.CoverMedia);

            var total = await baseQuery.CountAsync(ct);

            var posts = await baseQuery
                .OrderByDescending(p => p.PublishedAt ?? p.CreatedAt)
                .ThenByDescending(p => p.Id)
                .Skip((page - 1) * size)
                .Take(size)
                .ToListAsync(ct);

            var items = posts.Select(p =>
            {
                var loc = p.Locales.FirstOrDefault(l => l.Culture == culture)
                       ?? p.Locales.FirstOrDefault();
                var title = loc?.Title ?? "";
                var slug = loc?.Slug ?? p.SlugDefault;
                var content = loc?.Content ?? "";
                var excerpt = content.Length > 160 ? content[..160] + "…" : content;

                string? coverUrl = p.CoverMedia is null ? null
                    : $"{_opt.RequestPath}/{p.CoverMedia.StoredPath}".Replace("//", "/").Replace("\\", "/");

                return new BlogListItemVm(
                    p.Id, slug, title, excerpt, coverUrl, p.PublishedAt);
            })
            .Where(x => !string.IsNullOrWhiteSpace(x.Title))
            .ToList();

            return new PagedResult<BlogListItemVm>(total, page, size, items);
        }
    }
}
