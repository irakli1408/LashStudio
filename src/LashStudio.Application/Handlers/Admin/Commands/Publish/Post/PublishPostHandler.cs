using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Blog;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Publish.Post
{
    public sealed class PublishPostHandler : IRequestHandler<PublishPostCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IDateTime _clock; // уже есть в проекте

        public PublishPostHandler(IAppDbContext db, IDateTime clock)
        {
            _db = db;
            _clock = clock;
        }

        public async Task Handle(PublishPostCommand c, CancellationToken ct)
        {
            var post = await _db.Posts.FirstOrDefaultAsync(p => p.Id == c.PostId, ct)
                       ?? throw new KeyNotFoundException("post_not_found");

            if (c.Publish)
            {
                post.Status = PostStatus.Published;
                post.PublishedAt ??= _clock.UtcNow;
            }
            else
            {
                post.Status = PostStatus.Draft;
                post.PublishedAt = null;
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}
