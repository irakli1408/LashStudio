using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Delete
{
    public sealed class DeleteMediaAssetHandler : IRequestHandler<DeleteMediaAssetCommand>
    {
        private readonly IAppDbContext _db;
        private readonly IFileStorage _storage;

        public DeleteMediaAssetHandler(IAppDbContext db, IFileStorage storage)
        { _db = db; _storage = storage; }

        public async Task Handle(DeleteMediaAssetCommand c, CancellationToken ct)
        {
            var asset = await _db.MediaAssets
                .IgnoreQueryFilters()
                .Include(a => a.Attachments)
                .FirstOrDefaultAsync(a => a.Id == c.AssetId, ct)
                ?? throw new KeyNotFoundException("media_not_found");

            var attachedCount = asset.Attachments.Count;
            if (attachedCount > 0 && !c.Force)
                throw new InvalidOperationException($"media_in_use:{attachedCount}");

            // 1) Готовим изменения в БД
            if (attachedCount > 0)
                _db.MediaAttachments.RemoveRange(asset.Attachments);

            _db.MediaAssets.Remove(asset);

            // 2) Одно сохранение — EF сам откроет транзакцию и применит всё атомарно
            await _db.SaveChangesAsync(ct);

            // 3) После успешного коммита — чистим файлы в сторадже
            if (!string.IsNullOrWhiteSpace(asset.StoredPath))
                await _storage.DeleteAsync(asset.StoredPath, ct);

            if (!string.IsNullOrWhiteSpace(asset.PosterPath))
                await _storage.DeleteAsync(asset.PosterPath!, ct);
        }
    }
}
