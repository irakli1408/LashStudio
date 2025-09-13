using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Faq.GetById
{
    public record FaqLocaleVm(long Id, string Culture, string Question, string Answer);
    public record FaqItemAdminVm(long Id, bool IsActive, int SortOrder, List<FaqLocaleVm> Locales);

    public record GetFaqByIdQuery(long Id, string? Culture = null, bool OnlyThisCulture = false)
    : IRequest<FaqItemAdminVm>;
}
