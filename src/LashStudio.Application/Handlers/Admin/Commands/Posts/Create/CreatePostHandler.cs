using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Helpers;
using LashStudio.Application.Handlers.Admin.Commands.Posts.Create;
using LashStudio.Domain.Blog;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using static System.Net.WebRequestMethods;

public sealed class CreatePostHandler : IRequestHandler<CreatePostCommand, long>
{
    private readonly IAppDbContext _db;
    public CreatePostHandler(IAppDbContext db) => _db = db;

    public async Task<long> Handle(CreatePostCommand c, CancellationToken ct)
    {
        if (c.Locales is null || c.Locales.Count == 0)
            throw new ArgumentException("at_least_one_locale");

        // собираем пары (Culture, Slug), нормализуем
        var combos = c.Locales
            .Select(l => (
                Culture: l.Culture.Trim().ToLowerInvariant(),
                Slug: l.Slug.Trim()))
            .ToList();

        // CHANGED: строим выражения для каждой пары
        var exprs = combos.Select(cmb =>
            (Expression<Func<PostLocale, bool>>)(pl =>
                pl.Culture == cmb.Culture && pl.Slug == cmb.Slug));

        // CHANGED: объединяем условия через PredicateBuilder
        var predicate = PredicateBuilder.BuildOr(exprs);

        // CHANGED: проверка уникальности через готовое выражение
        var exists = await _db.PostLocales.AnyAsync(predicate, ct);
        if (exists)
            throw new ArgumentException("slug_exists");

        // создаём пост
        var post = new Post
        {
            SlugDefault = combos[0].Slug,  // первый slug используем как дефолт
            CoverMediaId = c.CoverMediaId,
            IsActive = c.IsActive            
        };

        // добавляем локализации
        foreach (var l in c.Locales)
        {
            post.Locales.Add(new PostLocale
            {
                Culture = l.Culture.Trim().ToLowerInvariant(),
                Title = l.Title.Trim(),
                Slug = l.Slug.Trim(),
                Content = (l.Content ?? "").Trim()
            });
        }

        _db.Posts.Add(post);
        await _db.SaveChangesAsync(ct);
        return post.Id;
    }
}
