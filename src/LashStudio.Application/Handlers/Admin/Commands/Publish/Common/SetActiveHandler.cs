using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Publish.Common
{
    public abstract class SetActiveHandler<TEntity, TKey>
     : IRequestHandler<SetActiveCommand<TEntity, TKey>, Unit>
     where TEntity : class, IHasId<TKey>, IActivatable
    {
        private readonly IAppDbContext _db;
        protected SetActiveHandler(IAppDbContext db) => _db = db;

        public async Task<Unit> Handle(SetActiveCommand<TEntity, TKey> c, CancellationToken ct)
        {
            var set = _db.Set<TEntity>();

            // быстрее и надёжнее для PK из одного столбца
            var entity = await set.FindAsync(new object?[] { c.Id }, ct)
                        ?? await set.FirstOrDefaultAsync(
                            e => EqualityComparer<TKey>.Default.Equals(e.Id, c.Id), ct)
                        ?? throw new KeyNotFoundException("not_found");

            entity.IsActive = c.Active;
            await _db.SaveChangesAsync(ct);
            return Unit.Value;
        }
    }
}