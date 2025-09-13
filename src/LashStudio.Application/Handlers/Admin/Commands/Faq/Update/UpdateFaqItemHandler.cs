using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Faq;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public sealed class UpdateFaqItemHandler : IRequestHandler<UpdateFaqItemCommand>
    {
        private readonly IAppDbContext _db;
        public UpdateFaqItemHandler(IAppDbContext db) => _db = db;

        public async Task Handle(UpdateFaqItemCommand c, CancellationToken ct)
        {
            var item = await _db.FaqItems
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.Id == c.Id, ct)
                ?? throw new KeyNotFoundException("faq_not_found");

            if (c.IsActive.HasValue) item.IsActive = c.IsActive.Value;
            if (c.SortOrder.HasValue) item.SortOrder = c.SortOrder.Value;

            if (c.Locales is { Count: > 0 })
            {
                foreach (var l in c.Locales)
                {
                    var culture = l.Culture.Trim().ToLowerInvariant();
                    var existing = item.Locales.FirstOrDefault(x => x.Culture == culture);
                    if (existing is null)
                    {
                        item.Locales.Add(new FaqItemLocale
                        {
                            Culture = culture,
                            Question = l.Question.Trim(),
                            Answer = l.Answer.Trim()
                        });
                    }
                    else
                    {
                        existing.Question = l.Question.Trim();
                        existing.Answer = l.Answer.Trim();
                    }
                }
                // Замечание: намеренно НЕ удаляем отсутствующие локали.
            }

            await _db.SaveChangesAsync(ct);
        }
    }
}