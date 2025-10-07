namespace LashStudio.Application.Contracts.Faq
{
    public sealed record FaqSortPairDto(long Id, int SortOrder);
    public sealed record FaqLocaleAdminVm(long Id, string Culture, string Question, string Answer);
    public sealed record FaqItemAdminVm(long Id, bool IsActive, int SortOrder, List<FaqLocaleAdminVm> Locales);
}
