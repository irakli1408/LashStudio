using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public sealed class GetServiceAdminDetailsHandler
        : IRequestHandler<GetServiceAdminDetailsQuery, ServiceAdminDto>
    {
        private readonly IAppDbContext _db;

        public GetServiceAdminDetailsHandler(IAppDbContext db) => _db = db;

        public async Task<ServiceAdminDto> Handle(GetServiceAdminDetailsQuery q, CancellationToken ct)
        {
            // 1. Берём сам сервис + локали
            var service = await _db.Services
                .AsNoTracking()
                .Include(s => s.Locales)
                .FirstOrDefaultAsync(s => s.Id == q.Id, ct)
                ?? throw new NotFoundException("service_not_found");

            var ownerKey = service.OwnerKey;

            // 2. Берём все медиа по OwnerKey, как в About
            var media = await _db.MediaAttachments
                .AsNoTracking()
                .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == ownerKey)
                .OrderBy(a => a.SortOrder)
                .Select(a => new ServiceMediaVm(
                    a.MediaAssetId,   // mediaAssetId
                    null,             // url (позже можно добавить через MediaAsset + UrlBuilder)
                    null,             // thumbUrl
                    null,             // contentType
                    a.SortOrder,      // sortOrder
                    a.IsCover,        // isCover
                    a.CreatedAtUtc    // createdAtUtc
                ))
                .ToListAsync(ct);

            // id обложки (как раньше)
            var coverMediaId = media.FirstOrDefault(m => m.IsCover)?.AssetId;

            // 3. Локали
            var locales = service.Locales
                .OrderBy(l => l.Culture)
                .Select(l => new ServiceLocaleDto(
                    l.Culture,
                    l.Title,
                    l.ShortDescription,
                    l.FullDescription
                ))
                .ToList();

            // 4. Финальный DTO
            return new ServiceAdminDto(
                service.Id,
                service.Slug,
                service.Category,
                service.Variant,
                service.Price,
                service.DurationMinutes,
                coverMediaId,
                service.IsActive,
                service.CreatedAtUtc,
                service.PublishedAtUtc,
                locales,
                media
            );
        }
    }
}
