using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Options;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Queries.Posts
{
    public sealed class GetPostByIdHandler : IRequestHandler<GetPostByIdQuery, PostAdminVm>
    {
        private readonly IAppDbContext _db;
        private readonly MediaOptions _media;

        public GetPostByIdHandler(IAppDbContext db, IOptions<MediaOptions> media)
        {
            _db = db;
            _media = media.Value;
        }

        public async Task<PostAdminVm> Handle(GetPostByIdQuery q, CancellationToken ct)
        {
            var post = await _db.Posts
                .AsNoTracking()
                .Include(p => p.Locales)
                .Include(p => p.CoverMedia)
                .FirstOrDefaultAsync(p => p.Id == q.Id, ct)
                ?? throw new KeyNotFoundException("post_not_found");

            string? coverUrl = post.CoverMedia is null ? null
                : $"{_media.RequestPath}/{post.CoverMedia.StoredPath}"
                    .Replace("//", "/").Replace("\\", "/");

            var locales = post.Locales
                .OrderBy(l => l.Culture)
                .Select(l => new PostLocaleAdminVm(l.Id, l.Culture, l.Title, l.Slug, l.Content))
                .ToList();

            return new PostAdminVm(
                post.Id,
                post.Status.ToString(),
                post.CreatedAt,
                post.PublishedAt,
                post.CoverMediaId,
                coverUrl,
                locales);
        }
    }
}
