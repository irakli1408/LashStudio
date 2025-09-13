using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Blog;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Update
{
    public sealed class UpdatePostHandler : IRequestHandler<UpdatePostCommand>
    {
        private readonly IAppDbContext _db;
        public UpdatePostHandler(IAppDbContext db) => _db = db;

        public async Task Handle(UpdatePostCommand c, CancellationToken ct)
        {
            var post = await _db.Posts
                .Include(p => p.Locales)
                .FirstOrDefaultAsync(p => p.Id == c.Id, ct)
                ?? throw new KeyNotFoundException("post_not_found");

            // обложка
            post.CoverMediaId = c.CoverMediaId;

            // upsert локалей (обновляем/добавляем, НЕ удаляем отсутствующие)
            foreach (var li in c.Locales)
            {
                var culture = li.Culture.Trim().ToLowerInvariant();
                var loc = post.Locales.FirstOrDefault(x => x.Culture == culture);
                if (loc is null)
                {
                    post.Locales.Add(new PostLocale
                    {
                        Culture = culture,
                        Title = li.Title,
                        Slug = li.Slug,
                        Content = li.Content
                    });
                }
                else
                {
                    loc.Title = li.Title;
                    loc.Slug = li.Slug;
                    loc.Content = li.Content;
                }
            }

            // базовый slug берём из первой локали
            var first = c.Locales.FirstOrDefault();
            if (first is not null)
                post.SlugDefault = first.Slug;

            await _db.SaveChangesAsync(ct);
        }
    }
}
