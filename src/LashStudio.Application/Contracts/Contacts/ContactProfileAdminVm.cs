namespace LashStudio.Application.Contracts.Contacts
{
    public sealed record ContactProfileAdminVm(
    long Id,
    string? EmailPrimary,
    string? EmailSales,
    string[] Phones,
    string? Instagram,
    string? Telegram,
    string? WhatsApp,
    decimal? MapLat,
    decimal? MapLng,
    int MapZoom,
    IReadOnlyList<ContactBusinessHourAdminVm> Hours,
    IReadOnlyList<ContactProfileLocaleVm> Locales,
    IReadOnlyList<ContactCtaAdminVm> Ctas,
    string? SeoTitle,
    string? SeoDescription);

    public sealed record ContactBusinessHourAdminVm(
        long Id, DayOfWeek Day, bool IsClosed, string? Open, string? Close);

    public sealed record ContactProfileLocaleVm(
        long Id, string Culture, string? OrganizationName, string? AddressLine, string? HowToFindUs);

}
