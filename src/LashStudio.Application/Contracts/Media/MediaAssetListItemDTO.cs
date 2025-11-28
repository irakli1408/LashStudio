using LashStudio.Domain.Media;

namespace LashStudio.Application.Contracts.Media
{
    public sealed record MediaAssetListItemVm(
    long Id,
    MediaType Type,
    string OriginalFileName,
    string StoredPath,
    string ContentType,
    long SizeBytes,
    DateTime CreatedAtUtc,
    string? Extension,
    bool IsDeleted,
    DateTime? DeletedAtUtc,
    int AttachmentsCount
);

    public sealed record MediaItemVm(
        long AssetId,
        string? Name,
        int MediaType,  // "image" | "video"
        string? Url,
        string? ThumbUrl
    );

    public sealed record MediaLibraryVm(
        //IReadOnlyList<MediaItemVm> All,
        IReadOnlyList<MediaItemVm> Photos,
        IReadOnlyList<MediaItemVm> Videos
    );
}
