using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Faq.GetById
{
    public sealed record GetFaqByIdQuery(long Id, string? Culture = null)
        : IRequest<FaqItemVm>;
}
