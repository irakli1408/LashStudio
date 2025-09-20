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

            // OwnerKey = строковое представление Id (long -> string) для общего media-флоу
            var ownerKey = about.Id.ToString();

            // Забираем привязанные медиа (позиционные аргументы в new AboutMediaVm(...))
            var media = await _db.MediaAttachments
                .AsNoTracking()
                .Where(m => m.OwnerType == MediaOwnerType.AboutPage && m.OwnerKey == ownerKey)
                .OrderBy(m => m.SortOrder)
                .Select(m => new AboutMediaVm(
                    m.MediaAssetId,   // MediaAssetId (long)
                    null,             // Url (если нужно — джойнить к таблице ассетов)
                    null,             // ThumbUrl
                    null,             // ContentType
                    m.SortOrder,      // SortOrder
                    m.IsCover,        // IsCover
                    m.CreatedAtUtc    // CreatedAtUtc
                ))
                .ToListAsync(ct);

            var locales = about.Locales
                .OrderBy(l => l.Culture)
                .Select(l => new AboutLocaleDto(
                    l.Culture,
                    l.Title,
                    l.SubTitle,
                    l.BodyHtml
                ))
                .ToList();

            return new AboutAdminDto(
                about.Id,
                about.IsActive,
                about.IsCover,
                about.CreatedAtUtc,
                about.PublishedAtUtc,
                about.SeoTitle,
                about.SeoDescription,
                about.SeoKeywordsCsv,
                locales,
                media
            );
        }
    }
}

