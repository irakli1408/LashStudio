using LashStudio.Application.Handlers.Admin.Queries.Faq.Get;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Faq.GetList
{
    public sealed record GetFaqListQuery(string? Culture = null) : IRequest<List<FaqItemVm>>;

}
