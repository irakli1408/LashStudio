using LashStudio.Domain.Media;

namespace LashStudio.Application.Contracts.AboutPerson
{
    public sealed record AboutLocaleDto(
        string Culture,
        string Title,
        string? SubTitle,
        string BodyHtml);

    public sealed record AboutMediaVm(
        long AssetId,
        string? Url,             // если у тебя есть таблица Asset c путями
        string? ThumbUrl,        // миниатюра, если генерируешь
        MediaType? MediaType,     // image/jpeg, video/mp4 ...
        int SortOrder,
        bool IsCover,
        DateTime CreatedAtUtc
    );

    public sealed record AboutAdminDto(
        long Id,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? PublishedAtUtc,
        long? CoverMediaId,
        List<AboutLocaleDto> Locales,
        List<AboutMediaVm> Media       // ← добавили
    );

    public sealed record AboutUpsertCommandDto(
        bool IsActive,
        List<AboutLocaleDto> Locales);
}

