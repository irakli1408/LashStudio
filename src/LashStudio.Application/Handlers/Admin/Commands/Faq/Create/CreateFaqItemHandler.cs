using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Faq;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Create;

public sealed class CreateFaqItemHandler : IRequestHandler<CreateFaqItemCommand, long>
{
    private readonly IAppDbContext _db;
    public CreateFaqItemHandler(IAppDbContext db) => _db = db;

    public async Task<long> Handle(CreateFaqItemCommand c, CancellationToken ct)
    {
        if (c.Locales is null || c.Locales.Count == 0)
            throw new ArgumentException("at_least_one_locale");

        var item = new FaqItem { IsActive = c.IsActive, SortOrder = c.SortOrder };

        foreach (var l in c.Locales)
        {
            if (string.IsNullOrWhiteSpace(l.Culture) ||
                string.IsNullOrWhiteSpace(l.Question) ||
                string.IsNullOrWhiteSpace(l.Answer))
                continue;

            item.Locales.Add(new FaqItemLocale
            {
                Culture = l.Culture.Trim().ToLowerInvariant(),
                Question = l.Question.Trim(),
                Answer = l.Answer.Trim()
            });
        }

        if (item.Locales.Count == 0) throw new ArgumentException("invalid_locales");

        _db.FaqItems.Add(item);
        await _db.SaveChangesAsync(ct);
        return item.Id;
    }
}
