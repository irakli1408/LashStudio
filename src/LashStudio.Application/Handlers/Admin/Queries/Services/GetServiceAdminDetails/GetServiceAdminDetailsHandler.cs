using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts.Services;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Common.Queries.ListMedia;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Services.GetServiceAdminDetails
{
    public sealed class GetServiceAdminDetailsHandler(IAppDbContext db, ISender _sender)
     : IRequestHandler<GetServiceAdminDetailsQuery, ServiceAdminDto>
    {
       
        public async Task<ServiceAdminDto> Handle(GetServiceAdminDetailsQuery q, CancellationToken ct)
        {
            var e = await db.Services.AsNoTracking()
                .Include(x => x.Locales)
                // .Include(x => x.Media)  // ← УБРАТЬ старую навигацию
                .FirstOrDefaultAsync(x => x.Id == q.Id, ct)
                ?? throw new NotFoundException("service_not_found");

            // ownerKey = GUID в каноническом виде
            var ownerKey = e.Id.ToString("D"); // можно .ToLowerInvariant(), если хочешь единый регистр

            // получаем вложения из общего флоу (через MediatR или сервис — как у тебя сделано)
            var attachments = await _sender.Send(new ListMediaQuery(MediaOwnerType.Service, ownerKey), ct);

            var media = attachments
                .OrderBy(a => a.SortOrder)
                .Select(a => new ServiceMediaVm(
                    MediaAssetId: a.MediaAssetId,
                    SortOrder: a.SortOrder,
                    IsCover: a.IsCover,
                    PosterAssetId: null)) // если у тебя постер не хранится отдельно
                .ToList();

            var coverId = attachments.FirstOrDefault(a => a.IsCover)?.MediaAssetId;

            return new ServiceAdminDto(
                e.Id, e.Slug, e.Category, e.Variant, e.Price, e.DurationMinutes,
                CoverMediaId: coverId,                              // теперь long?
                e.IsActive, e.CreatedAtUtc, e.PublishedAtUtc,
                e.Locales.Select(l => new ServiceLocaleDto(l.Culture, l.Title, l.ShortDescription, l.FullDescription)).ToList(),
                Media: media
            );
        }
    }

}
