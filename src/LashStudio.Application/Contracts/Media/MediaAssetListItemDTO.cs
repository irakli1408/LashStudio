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
}
