using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.AboutPerson;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.AboutPerson
{
    public sealed class GetAboutAdminHandler : IRequestHandler<GetAboutAdminQuery, AboutAdminDto>
    {
        private readonly IAppDbContext _db;
        public GetAboutAdminHandler(IAppDbContext db) => _db = db;

        public async Task<AboutAdminDto> Handle(GetAboutAdminQuery q, CancellationToken ct)
        {
            var about = await _db.AboutPages
                .AsNoTracking()
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(ct)
                ?? throw new NotFoundException("about_not_found", "about_not_found");

            var ownerKey = about.Id.ToString();

            // 1) Забираем media
            var media = await _db.MediaAttachments
                .AsNoTracking()
                .Where(m => m.OwnerType == MediaOwnerType.About && m.OwnerKey == ownerKey)
                .OrderBy(m => m.SortOrder)
                .Select(m => new AboutMediaVm(
                    m.MediaAssetId,
                    m.MediaAsset.StoredPath,
                    m.MediaAsset.ThumbStoredPath,
                    m.MediaAsset.Type,
                    m.SortOrder,
                    m.IsCover,
                    m.CreatedAtUtc
                ))
                .ToListAsync(ct);

            // 2) Находим cover (если есть)
            var coverMediaId = media.FirstOrDefault(m => m.IsCover)?.AssetId;

            // 3) Локали
            var locales = about.Locales
                .OrderBy(l => l.Culture)
                .Select(l => new AboutLocaleDto(
                    l.Culture,
                    l.Title,
                    l.SubTitle,
                    l.BodyHtml
                ))
                .ToList();

            // 4) Финальный DTO с CoverMediaId
            return new AboutAdminDto(
                Id: about.Id,
                IsActive: about.IsActive,
                CreatedAtUtc: about.CreatedAtUtc,
                PublishedAtUtc: about.PublishedAtUtc,
                CoverMediaId: coverMediaId,   
                Locales: locales,
                Media: media
            );
        }
    }
}
