using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Services.GetServicePublicDetails
{
    public sealed class GetServicePublicDetailsHandler
: IRequestHandler<GetServicePublicDetailsQuery, ServiceDetailsVm>
    {
        private readonly IAppDbContext _db;
        private readonly ICurrentStateService _state;

        public GetServicePublicDetailsHandler(IAppDbContext db, ICurrentStateService state)
        {
            _db = db;
            _state = state;
        }

        public async Task<ServiceDetailsVm> Handle(GetServicePublicDetailsQuery q, CancellationToken ct)
        {
            var culture = _state.CurrentCulture;
            var neutral = !string.IsNullOrWhiteSpace(culture) && culture!.Length >= 2 ? culture[..2] : null;

            // 1) Базовая инфа по услуге (без старой навигации Media)
            var svc = await _db.Services.AsNoTracking()
                .Where(s => s.Slug == q.Slug && s.IsActive)
                .Select(s => new
                {
                    s.Id,
                    s.Slug,
                    s.Price,
                    s.DurationMinutes,
                    Title = s.Locales
                        .OrderBy(l =>
                            l.Culture == culture ? 0 :
                            (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                        .Select(l => l.Title)
                        .FirstOrDefault(),
                    Description = s.Locales
                        .OrderBy(l =>
                            l.Culture == culture ? 0 :
                            (neutral != null && l.Culture.StartsWith(neutral)) ? 1 : 2)
                        .Select(l => l.FullDescription ?? l.ShortDescription)
                        .FirstOrDefault()
                })
                .FirstOrDefaultAsync(ct);

            if (svc is null)
                throw new NotFoundException("service_not_found", "service_not_found");

            // 2) Медиа из общего флоу
            var ownerKey = svc.Id.ToString("D"); // тот же формат, что и при attach/cover
            var media = await _db.MediaAttachments.AsNoTracking()
                .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == ownerKey)
                .OrderBy(a => a.SortOrder)
                .Select(a => new ServiceMediaVm(
                    a.MediaAssetId,      // long
                    a.SortOrder,
                    a.IsCover,
                    null                 // PosterAssetId: long? — если постер не храните отдельно
                ))
                .ToListAsync(ct);

            return new ServiceDetailsVm(
                svc.Id,
                svc.Slug,
                svc.Title ?? string.Empty,
                svc.Description,
                svc.Price,
                svc.DurationMinutes,
                media);
        }
    }
}

