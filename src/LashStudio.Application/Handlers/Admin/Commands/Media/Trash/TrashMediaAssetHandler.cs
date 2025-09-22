using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Trash
{
    public sealed class TrashMediaAssetHandler : IRequestHandler<TrashMediaAssetCommand>
    {
        private readonly IAppDbContext _db;
        public TrashMediaAssetHandler(IAppDbContext db) => _db = db;

        public async Task Handle(TrashMediaAssetCommand c, CancellationToken ct)
        {
            var a = await _db.MediaAssets
                .IgnoreQueryFilters() // чтобы найти даже если уже в корзине
                .FirstOrDefaultAsync(x => x.Id == c.AssetId, ct)
                ?? throw new KeyNotFoundException("media_not_found");

            if (!a.IsDeleted)
            {
                a.IsDeleted = true;
                a.DeletedAtUtc = DateTime.UtcNow;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
