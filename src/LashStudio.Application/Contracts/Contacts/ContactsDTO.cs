using LashStudio.Domain.Contacts;

namespace LashStudio.Application.Contracts.Contacts
{
    public record ContactProfileLocaleDto(string Culture, string? OrganizationName, string? AddressLine, string? HowToFindUs);

    public record ContactBusinessHourDto(DayOfWeek Day, bool IsClosed, string? Open, string? Close);
    // Open/Close как "HH:mm" строки, чтобы удобнее с фронта.

    public record ContactProfileUpsertDto(
        string? EmailPrimary, string? EmailSales, string[] Phones,
        string? Instagram, string? Telegram, string? WhatsApp,
        decimal? MapLat, decimal? MapLng, int MapZoom,
        CtaKind PreferredCta,
        IReadOnlyList<ContactBusinessHourDto>? Hours,                 // ОПЦИОНАЛЬНО
        IReadOnlyList<ContactProfileLocaleDto> Locales,               // хотя бы одна локаль желательна
        string? SeoTitle, string? SeoDescription);

    public record BusinessHourVm(DayOfWeek Day, bool IsClosed, string? Open, string? Close);

    public record CtaVm(string Kind, string Label, string Url);

    public record ContactProfileVm(
        string? OrganizationName, string? Address, string? HowToFindUs,
        string[] Phones, string? EmailPrimary, string? EmailSales,
        string? Instagram, string? Telegram, string? WhatsApp,
        decimal? MapLat, decimal? MapLng, int MapZoom,
        IReadOnlyList<BusinessHourVm>? Hours, // ОПЦИОНАЛЬНО
        IReadOnlyList<CtaVm> Ctas,
        string? SeoTitle, string? SeoDescription);

    public record ContactMessageCreateDto(
        string? Name, string? Phone, string? Email, string? Subject, string Body,
        bool ConsentAccepted, string? CaptchaToken);

    public record ContactMessageVm(
        long Id, DateTime CreatedAtUtc, string? Name, string? Phone, string? Email,
        string? Subject, string Body, ContactMessageStatus Status);

    public record ContactMessageFilter(ContactMessageStatus? Status, int Page = 1, int PageSize = 20);

    public record SetContactMessageStatusDto(ContactMessageStatus Status);
}
