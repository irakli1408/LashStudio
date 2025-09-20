using LashStudio.Domain.Media;

namespace LashStudio.Application.Common.Abstractions
{
    public interface IMediaAttachmentService
    {
        Task AttachAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct);
        Task DetachAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct);
        Task ReorderAsync(MediaOwnerType ownerType, string ownerKey, IReadOnlyList<long> assetIdsInOrder, CancellationToken ct);
        Task SetCoverAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct);
        Task<IReadOnlyList<MediaAttachment>> ListAsync(MediaOwnerType ownerType, string ownerKey, CancellationToken ct);
    }
}
