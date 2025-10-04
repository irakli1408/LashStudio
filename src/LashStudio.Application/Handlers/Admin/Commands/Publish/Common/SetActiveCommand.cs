using LashStudio.Domain.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Publish.Common
{
    public sealed record SetActiveCommand<TEntity, TKey>(TKey Id, bool Active) : IRequest<Unit>
    where TEntity : class, IHasId<TKey>, IActivatable;

}
