using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Helpers;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public sealed class GetServiceAdminDetailsHandler
        : IRequestHandler<GetServiceAdminDetailsQuery, ServiceAdminDto>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;

        public GetServiceAdminDetailsHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        {
            _db = db;
            _opt = opt;
        }

        public async Task<ServiceAdminDto> Handle(GetServiceAdminDetailsQuery q, CancellationToken ct)
        {
            // 1. Берём сам сервис + локали
            var service = await _db.Services
                .AsNoTracking()
                .Include(s => s.Locales)
                .FirstOrDefaultAsync(s => s.Id == q.Id, ct)
                ?? throw new NotFoundException("service_not_found");

            var ownerKey = service.OwnerKey;

            // 2. Берём все медиа по OwnerKey (сырые данные из MediaAsset)
            var mediaRaw = await _db.MediaAttachments
                .AsNoTracking()
                .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == ownerKey)
                .OrderBy(a => a.SortOrder)
                .Select(a => new
                {
                    a.MediaAssetId,
                    a.SortOrder,
                    a.IsCover,
                    a.CreatedAtUtc,
                    a.MediaAsset.StoredPath,
                    a.MediaAsset.ThumbStoredPath,
                    a.MediaAsset.Type
                })
                .ToListAsync(ct);

            // 3. Собираем VM и строим Url в памяти
            var media = mediaRaw
                .Select(a => new ServiceMediaVm(
                    AssetId: a.MediaAssetId,
                    Url: MediaUrlHelper.ToUrl(_opt.Value, a.StoredPath),
                    ThumbUrl: a.ThumbStoredPath is null
                        ? null
                        : MediaUrlHelper.ToUrl(_opt.Value, a.ThumbStoredPath),
                    MediaType: a.Type,
                    SortOrder: a.SortOrder,
                    IsCover: a.IsCover,
                    CreatedAtUtc: a.CreatedAtUtc
                ))
                .ToList();

            // id обложки
            var coverMediaId = media.FirstOrDefault(m => m.IsCover)?.AssetId;

            // 4. Локали
            var locales = service.Locales
                .OrderBy(l => l.Culture)
                .Select(l => new ServiceLocaleDto(
                    l.Culture,
                    l.Title,
                    l.ShortDescription,
                    l.FullDescription
                ))
                .ToList();

            // 5. Финальный DTO
            return new ServiceAdminDto(
                Id: service.Id,
                Slug: service.Slug,
                Category: service.Category,
                Variant: service.Variant,
                Price: service.Price,
                DurationMinutes: service.DurationMinutes,
                CoverMediaId: coverMediaId,
                IsActive: service.IsActive,
                CreatedAtUtc: service.CreatedAtUtc,
                PublishedAtUtc: service.PublishedAtUtc,
                Locales: locales,
                Media: media
            );
        }
    }
}
