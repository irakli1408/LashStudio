namespace LashStudio.Application.Contracts.AboutPerson
{
    public sealed record PublicMediaVm(
    long MediaAssetId,
    int SortOrder,
    bool IsCover,
    DateTime CreatedAtUtc
    // при желании добавь Url/Thumb/ContentType, если есть таблица ассетов
);

    public sealed record AboutPublicVm(
        string Title,
        string? SubTitle,
        string BodyHtml,
        IReadOnlyList<PublicMediaVm> Media
    );
}
