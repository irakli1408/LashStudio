using LashStudio.Domain.Contacts;

namespace LashStudio.Api.Contracts.Contacts
{
    public sealed record ContactCtaLocaleUpsertDto(string Culture, string Label);
    public sealed record ContactCtaCreateDto(CtaKind Kind, bool IsEnabled, int Order, string? UrlOverride, IReadOnlyList<ContactCtaLocaleUpsertDto> Locales);
    public sealed record ContactCtaUpdateDto(long Id, CtaKind Kind, bool IsEnabled, string? UrlOverride);
    public sealed record ReorderContactCtasDto(IReadOnlyList<long> OrderedIds);
    public sealed record SetCtaEnabledDto(bool IsEnabled);

    public sealed record ContactCtaAdminVm(long Id, CtaKind Kind, bool IsEnabled, int Order, string? UrlOverride, IReadOnlyList<ContactCtaLocaleVm> Locales);
    public sealed record ContactCtaLocaleVm(long Id, string Culture, string Label);

}
