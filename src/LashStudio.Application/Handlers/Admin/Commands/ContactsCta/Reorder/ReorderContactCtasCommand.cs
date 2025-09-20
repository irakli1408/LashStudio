using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Reorder
{
    public sealed record ReorderContactCtasCommand(IReadOnlyList<long> OrderedIds) : IRequest<Unit>;
}
