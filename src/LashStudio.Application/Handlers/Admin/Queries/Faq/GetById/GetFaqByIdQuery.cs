using LashStudio.Application.Contracts.Faq;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Faq.GetById
{
    public sealed record GetFaqByIdQuery(long Id, string? Culture = null, bool OnlyThisCulture = false)
        : IRequest<FaqItemAdminVm>;
}
