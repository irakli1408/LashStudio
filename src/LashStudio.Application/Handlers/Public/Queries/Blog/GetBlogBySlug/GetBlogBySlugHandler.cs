using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogBySlug
{
    public sealed class GetBlogBySlugHandler : IRequestHandler<GetBlogBySlugQuery, BlogPostVm>
    {
        private readonly IAppDbContext _db;
        private readonly MediaOptions _opt;
        public GetBlogBySlugHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        { _db = db; _opt = opt.Value; }

        public async Task<BlogPostVm> Handle(GetBlogBySlugQuery q, CancellationToken ct)
        {
            var culture = q.Culture.Trim().ToLowerInvariant();
            var slug = q.Slug.Trim();

            var pl = await _db.PostLocales
                .AsNoTracking()
                .Include(x => x.Post)
                    .ThenInclude(p => p.CoverMedia)
                .FirstOrDefaultAsync(x =>
                    x.Culture == culture &&
                    x.Slug == slug &&
                    x.Post.Status == Domain.Blog.PostStatus.Published, ct)
                ?? throw new KeyNotFoundException("post_not_found");

            string? coverUrl = pl.Post.CoverMedia is null ? null
                : $"{_opt.RequestPath}/{pl.Post.CoverMedia.StoredPath}".Replace("//", "/").Replace("\\", "/");

            return new BlogPostVm(
                pl.PostId, pl.Slug, pl.Title, pl.Content,
                coverUrl, pl.Post.PublishedAt);
        }
    }
}
