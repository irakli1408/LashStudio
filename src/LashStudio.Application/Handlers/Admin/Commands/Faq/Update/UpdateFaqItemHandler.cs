using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Faq;
using LashStudio.Domain.Faq;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public sealed class UpdateFaqItemHandler : IRequestHandler<UpdateFaqItemCommand, FaqItemAdminVm>
    {
        private readonly IAppDbContext _db;
        public UpdateFaqItemHandler(IAppDbContext db) => _db = db;

        public async Task<FaqItemAdminVm> Handle(UpdateFaqItemCommand r, CancellationToken ct)
        {
            var d = r.Body;

            var item = await _db.FaqItems
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.Id == d.Id, ct)
                ?? throw new KeyNotFoundException("faq_not_found");

            // PUT-семантика: перезаписываем основные поля
            item.IsActive = d.IsActive;
            item.SortOrder = d.SortOrder;

            // Апсерт локалей: по Id, иначе по Culture
            if (d.Locales is { Count: > 0 })
            {
                foreach (var l in d.Locales)
                {
                    var culture = l.Culture?.Trim().ToLowerInvariant() ?? "";

                    FaqItemLocale? existing = null;
                    if (l.Id > 0)
                        existing = item.Locales.FirstOrDefault(x => x.Id == l.Id);

                    existing ??= item.Locales.FirstOrDefault(x => x.Culture == culture);

                    if (existing is null)
                    {
                        item.Locales.Add(new FaqItemLocale
                        {
                            Culture = culture,
                            Question = l.Question?.Trim() ?? "",
                            Answer = l.Answer?.Trim() ?? ""
                        });
                    }
                    else
                    {
                        existing.Culture = culture;
                        existing.Question = l.Question?.Trim() ?? "";
                        existing.Answer = l.Answer?.Trim() ?? "";
                    }
                }

                // (опционально, для строгого PUT) удалить локали, которых нет в body:
                // var keepIds = d.Locales.Where(x => x.Id > 0).Select(x => x.Id).ToHashSet();
                // item.Locales.RemoveAll(x => x.Id > 0 && !keepIds.Contains(x.Id));
            }

            await _db.SaveChangesAsync(ct);

            // Возвращаем обновлённый VM (тот же формат)
            var vm = new FaqItemAdminVm(
                item.Id,
                item.IsActive,
                item.SortOrder,
                item.Locales
                    .OrderBy(x => x.Culture)
                    .Select(x => new FaqLocaleAdminVm(x.Id, x.Culture, x.Question, x.Answer))
                    .ToList()
            );

            return vm;
        }
    }
}