using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Faq;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Reorder
{
    public sealed class ReorderFaqItemsHandler : IRequestHandler<ReorderFaqItemsCommand>
    {
        private readonly IAppDbContext _db;
        public ReorderFaqItemsHandler(IAppDbContext db) => _db = db;

        public async Task Handle(ReorderFaqItemsCommand r, CancellationToken ct)
        {
            // Берём только те элементы, которые реально меняем
            var ids = r.Pairs.Select(p => p.Id).ToArray();

            var items = await _db.Set<FaqItem>()
                .Where(x => ids.Contains(x.Id))
                .ToListAsync(ct);

            if (items.Count != ids.Length)
            {
                var foundIds = items.Select(i => i.Id).ToHashSet();
                var missing = ids.Where(id => !foundIds.Contains(id)).ToArray();
                // не раскрываем лишнюю инфу? — здесь полезно явно сказать какие не нашлись
                throw new KeyNotFoundException($"faq_items_not_found: {string.Join(',', missing)}");
            }

            // Обновляем сортировки
            var byId = r.Pairs.ToDictionary(p => p.Id, p => p.SortOrder);
            foreach (var item in items)
                item.SortOrder = byId[item.Id];

            await _db.SaveChangesAsync(ct);
        }
    }
}
