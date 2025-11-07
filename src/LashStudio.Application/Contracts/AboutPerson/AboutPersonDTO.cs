namespace LashStudio.Application.Contracts.AboutPerson
{
    public sealed record AboutLocaleDto(
        string Culture,
        string Title,
        string? SubTitle,
        string BodyHtml);

    public sealed record AboutMediaVm(
        long MediaAssetId,
        string? Url,             // если у тебя есть таблица Asset c путями
        string? ThumbUrl,        // миниатюра, если генерируешь
        string? ContentType,     // image/jpeg, video/mp4 ...
        int SortOrder,
        bool IsCover,
        DateTime CreatedAtUtc
    );

    public sealed record AboutAdminDto(
        long Id,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? PublishedAtUtc,
        string? SeoTitle,
        string? SeoDescription,
        string? SeoKeywordsCsv,
        List<AboutLocaleDto> Locales,
        List<AboutMediaVm> Media       // ← добавили
    );

    public sealed record AboutUpsertCommandDto(
        bool IsActive,
        string? SeoTitle,
        string? SeoDescription,
        string? SeoKeywordsCsv,
        List<AboutLocaleDto> Locales);

}

