using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Media.Restore
{
    public sealed class RestoreMediaAssetHandler : IRequestHandler<RestoreMediaAssetCommand>
    {
        private readonly IAppDbContext _db;
        public RestoreMediaAssetHandler(IAppDbContext db) => _db = db;

        public async Task Handle(RestoreMediaAssetCommand c, CancellationToken ct)
        {
            var a = await _db.MediaAssets
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(x => x.Id == c.AssetId, ct)
                ?? throw new KeyNotFoundException("media_not_found");

            if (a.IsDeleted)
            {
                a.IsDeleted = false;
                a.DeletedAtUtc = null;
                await _db.SaveChangesAsync(ct);
            }
        }
    }
}
