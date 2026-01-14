using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace LashStudio.Infrastructure.Config.Media
{
    public sealed class MediaAttachmentService : IMediaAttachmentService
    {
        private readonly IAppDbContext _db;
        public MediaAttachmentService(IAppDbContext db) => _db = db;

        public async Task AttachAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
        {
            var exists = await _db.MediaAssets.AnyAsync(a => a.Id == assetId, ct);
            if (!exists) throw new NotFoundException("asset_not_found", "asset_not_found");

            var dup = await _db.MediaAttachments
                .AnyAsync(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey && x.MediaAssetId == assetId, ct);
            if (dup) return;

            var maxOrder = await _db.MediaAttachments
                .Where(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey)
                .Select(x => (int?)x.SortOrder).MaxAsync(ct) ?? -1;

            _db.MediaAttachments.Add(new MediaAttachment
            {
                OwnerType = ownerType,
                OwnerKey = ownerKey,
                MediaAssetId = assetId,
                SortOrder = maxOrder + 1,
                CreatedAtUtc = DateTime.UtcNow
            });

            await _db.SaveChangesAsync(ct);
        }

        public async Task DetachAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
        {
            var items = await _db.MediaAttachments
                .Where(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            var removed = items.FirstOrDefault(x => x.MediaAssetId == assetId)
                ?? throw new NotFoundException("media_attachment_not_found", "media_attachment_not_found");

            _db.MediaAttachments.Remove(removed);
            items.Remove(removed);

            for (int i = 0; i < items.Count; i++)
                items[i].SortOrder = i;

            await _db.SaveChangesAsync(ct);
        }

        public async Task ReorderAsync(MediaOwnerType ownerType, string ownerKey, IReadOnlyList<long> assetIdsInOrder, CancellationToken ct)
        {
            var items = await _db.MediaAttachments
                .Where(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey)
                .ToListAsync(ct);

            var setDb = items.Select(x => x.MediaAssetId).OrderBy(x => x).ToArray();
            var setReq = assetIdsInOrder.OrderBy(x => x).ToArray();
            if (!setDb.SequenceEqual(setReq))
                throw new ValidationException("asset_set_mismatch");

            for (int i = 0; i < assetIdsInOrder.Count; i++)
                items.First(x => x.MediaAssetId == assetIdsInOrder[i]).SortOrder = i;

            await _db.SaveChangesAsync(ct);
        }

        public async Task SetCoverAsync(MediaOwnerType ownerType, string ownerKey, long assetId, CancellationToken ct)
        {
            var items = await _db.MediaAttachments
                .Where(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey)
                .ToListAsync(ct);

            var newCover = items.SingleOrDefault(x => x.MediaAssetId == assetId);
            if (newCover is null)
                throw new NotFoundException("asset_not_attached", "asset_not_attached");


            var changed = false;
            foreach (var i in items)
            {
                if (i.IsCover && i.Id != newCover.Id)
                {
                    i.IsCover = false;
                    changed = true;
                }
            }

            if (changed)
                await _db.SaveChangesAsync(ct); 

            if (!newCover.IsCover)
            {
                newCover.IsCover = true;
                await _db.SaveChangesAsync(ct);
            }
        }



        public Task<IReadOnlyList<MediaAttachment>> ListAsync(MediaOwnerType ownerType, string ownerKey, CancellationToken ct)
            => _db.MediaAttachments
                  .Where(x => x.OwnerType == ownerType && x.OwnerKey == ownerKey)
                  .OrderBy(x => x.SortOrder)
                  .ToListAsync(ct)
                  .ContinueWith(t => (IReadOnlyList<MediaAttachment>)t.Result, ct);
    }
}


