using LashStudio.Domain.Services;

namespace LashStudio.Application.Contracts.Services
{
    public record ServiceLocaleDto(string Culture, string Title, string? ShortDescription, string? FullDescription);

    // было Guid → стало long
    public record ServiceMediaVm(
        long MediaAssetId,
        string? Url,             // если у тебя есть таблица Asset c путями
        string? ThumbUrl,        // миниатюра, если генерируешь
        string? ContentType,     // image/jpeg, video/mp4 ...
        int SortOrder,
        bool IsCover,
        DateTime CreatedAtUtc);

    // было Guid? → стало long?
    public record ServiceAdminDto(
        Guid Id, string Slug, ServiceCategory Category, LashExtensionVariant? Variant,
        decimal Price, int? DurationMinutes, long? CoverMediaId,
        bool IsActive, DateTime CreatedAtUtc, DateTime? PublishedAtUtc,
        List<ServiceLocaleDto> Locales, List<ServiceMediaVm> Media);

    // этот VM можно оставить без изменений
    public record ServiceListItemVm(
        Guid Id, string Slug, string Title, decimal Price, string? CoverUrl);

    public sealed record ServiceListItemWithMediaVm(
    Guid Id,
    string Slug,
    string Title,
    decimal Price,
    List<ServiceMediaVm> Media
);

    public record ServiceDetailsVm(
        Guid Id, string Slug, string Title, string? Description, decimal Price, int? DurationMinutes,
        List<ServiceMediaVm> Media);

}
