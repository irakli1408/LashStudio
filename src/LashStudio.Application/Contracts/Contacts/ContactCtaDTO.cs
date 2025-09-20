using LashStudio.Domain.Contacts;

namespace LashStudio.Application.Contracts.Contacts
{
    public sealed record ContactCtaLocaleUpsertDto(
    string Culture,
    string Label);

    public sealed record ContactCtaCreateDto(
        CtaKind Kind,
        bool IsEnabled,
        int Order,
        string? UrlOverride,
        IReadOnlyList<ContactCtaLocaleUpsertDto> Locales);

    public sealed record ContactCtaUpdateDto(
        long Id,
        CtaKind Kind,
        bool IsEnabled,
        string? UrlOverride);

    public sealed record ReorderContactCtasDto(
        IReadOnlyList<long> OrderedIds);

    public sealed record SetCtaEnabledDto(
        bool IsEnabled);

    public sealed record ContactCtaAdminVm(
        long Id,
        CtaKind Kind,
        bool IsEnabled,
        int Order,
        string? UrlOverride,
        IReadOnlyList<ContactCtaLocaleVm> Locales);

    public sealed record ContactCtaLocaleVm(
        long Id,
        string Culture,
        string Label);
}

public sealed record ContactProfileLocaleDto(
    string Culture,
    string? OrganizationName,
    string? AddressLine,
    string? HowToFindUs);

public sealed record ContactBusinessHourDto(
    DayOfWeek Day,
    bool IsClosed,
    string? Open,   // "HH:mm" или null
    string? Close); // "HH:mm" или null

public sealed record ContactProfileUpsertDto(
    string? EmailPrimary,
    string? EmailSales,
    string[] Phones,
    string? Instagram,
    string? Telegram,
    string? WhatsApp,
    decimal? MapLat,
    decimal? MapLng,
    int MapZoom,
    IReadOnlyList<ContactBusinessHourDto>? Hours,            // ← опционально
    IReadOnlyList<ContactProfileLocaleDto> Locales,          // ← хотя бы одна
    string? SeoTitle,
    string? SeoDescription);
