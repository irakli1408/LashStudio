using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Helpers;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Contracts.Services;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Linq;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminList
{
    public sealed class GetServiceAdminListHandler
        : IRequestHandler<GetServiceAdminListQuery, List<ServiceListItemWithMediaVm>>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;
        private readonly ICurrentStateService _state;

        public GetServiceAdminListHandler(
            IAppDbContext db,
            ICurrentStateService state,
            IOptions<MediaOptions> opt)
        {
            _db = db;
            _state = state;
            _opt = opt;
        }

        public async Task<List<ServiceListItemWithMediaVm>> Handle(
     GetServiceAdminListQuery q,
     CancellationToken ct)
        {
            var culture = _state.CurrentCulture;
            var neutral = !string.IsNullOrEmpty(culture) && culture.Length >= 2
                ? culture[..2]
                : null;

            var baseQuery = _db.Services
                .AsNoTracking()
                .Where(s => !q.Category.HasValue || s.Category == q.Category)
                .Select(s => new
                {
                    s.Id,
                    s.Slug,
                    s.Price,
                    s.OwnerKey,
                    Title = s.Locales
                        .OrderBy(l =>
                            l.Culture == culture ? 0 :
                            (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                        .Select(l => l.Title)
                        .FirstOrDefault() ?? ""
                });

            var ordered = baseQuery.OrderBy(x => x.Title);

            var raw = await ordered
                .Select(x => new
                {
                    x.Id,
                    x.Slug,
                    x.Title,
                    x.Price,
                    Media = _db.MediaAttachments
                        .Where(m => m.OwnerType == MediaOwnerType.Service && m.OwnerKey == x.OwnerKey)
                        .OrderBy(m => m.SortOrder)
                        .Select(m => new
                        {
                            m.MediaAssetId,
                            m.MediaAsset.StoredPath,
                            m.MediaAsset.ThumbStoredPath,
                            Type = (int)m.MediaAsset.Type,
                            m.SortOrder,
                            m.IsCover,
                            m.CreatedAtUtc
                        })
                        .ToList()
                })
                .ToListAsync(ct);

            var result = raw.Select(x =>
            {
                var mediaList = x.Media
                    .Select(m => new ServiceMediaVm(
                        AssetId: m.MediaAssetId,
                        Url: MediaUrlHelper.ToUrl(_opt.Value, m.StoredPath),
                        ThumbUrl: m.ThumbStoredPath is null
                            ? null
                            : MediaUrlHelper.ToUrl(_opt.Value, m.ThumbStoredPath),
                        MediaType: (MediaType)m.Type,
                        SortOrder: m.SortOrder,
                        IsCover: m.IsCover,
                        CreatedAtUtc: m.CreatedAtUtc
                    ))
                    .ToList();

                var coverMediaId = mediaList.FirstOrDefault(mm => mm.IsCover)?.AssetId;

                return new ServiceListItemWithMediaVm(
                    Id: x.Id,
                    Slug: x.Slug,
                    Title: x.Title,
                    Price: x.Price,
                    CoverMediaId: coverMediaId,
                    Media: mediaList
                );
            })
            .ToList();

            return result; 
        }
    }
}