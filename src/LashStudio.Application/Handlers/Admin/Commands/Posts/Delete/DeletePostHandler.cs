using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Delete
{
    public sealed class DeletePostHandler : IRequestHandler<DeletePostCommand>
    {
        private readonly IAppDbContext _db;
        public DeletePostHandler(IAppDbContext db) => _db = db;

        public async Task Handle(DeletePostCommand c, CancellationToken ct)
        {
            var post = await _db.Posts
                .Include(p => p.Locales)
                .FirstOrDefaultAsync(p => p.Id == c.Id, ct)
                ?? throw new KeyNotFoundException("post_not_found");

            _db.Posts.Remove(post);
            await _db.SaveChangesAsync(ct);
        }
    }
}
