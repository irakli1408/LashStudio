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

            // базовый запрос по сервисам
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

            // 1) Забираем из БД "сырые" данные (без ToUrl)
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

            // 2) В памяти мапим в итоговые VM и прогоняем через ToUrl
            var result = raw
             .Select(x => new ServiceListItemWithMediaVm(
                 x.Id,
                 x.Slug,
                 x.Title,
                 x.Price,
                 x.Media
                     .Select(m => new ServiceMediaVm(
                         m.MediaAssetId,                                       // mediaAssetId
                         MediaUrlHelper.ToUrl(_opt.Value, m.StoredPath),                                  // url
                         m.ThumbStoredPath is null ? null : MediaUrlHelper.ToUrl(_opt.Value, m.ThumbStoredPath), // thumbUrl (может быть null)
                         (MediaType)m.Type,                                    // contentType / type
                         m.SortOrder,                                          // sortOrder
                         m.IsCover,                                            // isCover
                         m.CreatedAtUtc                                        // createdAtUtc
                     ))
                     .ToList()
             ))
             .ToList();

            return result;
        }
    }
}
