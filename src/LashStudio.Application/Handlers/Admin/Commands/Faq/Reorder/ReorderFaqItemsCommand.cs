using LashStudio.Application.Contracts.Faq;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Reorder
{
    public sealed record ReorderFaqItemsCommand(IReadOnlyList<FaqSortPairDto> Pairs) : IRequest;
}
