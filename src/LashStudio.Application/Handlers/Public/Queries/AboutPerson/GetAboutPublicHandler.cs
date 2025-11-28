using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.AboutPerson;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.AboutPerson
{
    public sealed class GetAboutPublicHandler : IRequestHandler<GetAboutPublicQuery, AboutPublicVm>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state; 

        public GetAboutPublicHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db; _state = state;
        }

        public async Task<AboutPublicVm> Handle(GetAboutPublicQuery q, CancellationToken ct)
        {
            var culture = string.IsNullOrWhiteSpace(q.Culture) ? _state.CurrentCulture : q.Culture!;
            var neutral = !string.IsNullOrWhiteSpace(culture) && culture!.Length >= 2 ? culture[..2] : null;

            var about = await _db.AboutPages
                .AsNoTracking()
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.IsActive, ct)
                ?? throw new NotFoundException("about_not_found", "about_not_found");

            var locale = about.Locales
                .OrderBy(l =>
                    l.Culture == culture ? 0 :
                    (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                .FirstOrDefault()
                ?? throw new NotFoundException("locale_not_found", "locale_not_found");

            var ownerKey = about.Id.ToString();

            var media = await _db.MediaAttachments
                .AsNoTracking()
                .Where(m => m.OwnerType == MediaOwnerType.About && m.OwnerKey == ownerKey)
                .OrderBy(m => m.SortOrder)
                .Select(m => new PublicMediaVm(
                    m.MediaAssetId,
                    m.SortOrder,
                    m.IsCover,
                    m.CreatedAtUtc
                ))
                .ToListAsync(ct);

            return new AboutPublicVm(
                locale.Title,
                locale.SubTitle,
                locale.BodyHtml,
                media
            );
        }
    }
}
