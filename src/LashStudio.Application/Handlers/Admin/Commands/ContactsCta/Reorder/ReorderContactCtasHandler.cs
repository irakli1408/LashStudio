using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Reorder
{
    public sealed class ReorderContactCtasHandler
    : IRequestHandler<ReorderContactCtasCommand, Unit>
    {
        private readonly IAppDbContext _db;
        public ReorderContactCtasHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(ReorderContactCtasCommand r, CancellationToken ct)
        {
            var orderMap = r.OrderedIds.Select((id, idx) => (id, idx)).ToDictionary(t => t.id, t => t.idx);
            var items = await _db.ContactCtas.Where(x => r.OrderedIds.Contains(x.Id)).ToListAsync(ct);

            foreach (var i in items)
                if (orderMap.TryGetValue(i.Id, out var ord)) i.Order = ord;

            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}
