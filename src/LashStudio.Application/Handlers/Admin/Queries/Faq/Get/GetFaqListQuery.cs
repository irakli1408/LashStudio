using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Faq.Get
{
    public record FaqItemVm(long Id, int SortOrder, string Question, string Answer);

    public sealed record GetFaqListQuery(string Culture) : IRequest<List<FaqItemVm>>;
}
