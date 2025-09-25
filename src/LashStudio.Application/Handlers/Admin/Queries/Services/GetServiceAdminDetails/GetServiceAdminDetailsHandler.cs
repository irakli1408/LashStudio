using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public sealed class GetServiceAdminDetailsHandler(IAppDbContext _db)
    : IRequestHandler<GetServiceAdminDetailsQuery, ServiceAdminDto>
    {
        public async Task<ServiceAdminDto> Handle(GetServiceAdminDetailsQuery q, CancellationToken ct)
        {
            var dto = await _db.Services.AsNoTracking()
                .Where(s => s.Id == q.Id)
                .Select(s => new ServiceAdminDto(
                    s.Id,
                    s.Slug,
                    s.Category,
                    s.Variant,
                    s.Price,
                    s.DurationMinutes,
                    _db.MediaAttachments
                        .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == s.OwnerKey && a.IsCover)
                        .Select(a => a.MediaAssetId) 
                        .FirstOrDefault(),
                    s.IsActive,
                    s.CreatedAtUtc,
                    s.PublishedAtUtc,
                    s.Locales
                        .OrderBy(l => l.Culture)
                        .Select(l => new ServiceLocaleDto(l.Culture, l.Title, l.ShortDescription, l.FullDescription))
                        .ToList(),
                    _db.MediaAttachments
                        .Where(a => a.OwnerType == MediaOwnerType.Service && a.OwnerKey == s.OwnerKey)
                        .OrderBy(a => a.SortOrder)
                        .Select(a => new ServiceMediaVm(a.MediaAssetId, a.SortOrder, a.IsCover, null))
                        .ToList()
                ))
                .FirstOrDefaultAsync(ct);

            if (dto is null) throw new NotFoundException("service_not_found");
            return dto;
        }
    }
}
