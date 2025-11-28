using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Helpers;
using LashStudio.Application.Common.Options;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminDetails;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails
{
    public sealed class GetCourseAdminDetailsHandler
    : IRequestHandler<GetCourseAdminDetailsQuery, CourseAdminDto>
    {
        private readonly IAppDbContext _db;
        private readonly IOptions<MediaOptions> _opt;

        public GetCourseAdminDetailsHandler(IAppDbContext db, IOptions<MediaOptions> opt)
        {
            _db = db;
            _opt = opt;
        }

        public async Task<CourseAdminDto> Handle(GetCourseAdminDetailsQuery q, CancellationToken ct)
        {
            var row = await _db.Courses.AsNoTracking()
                .Where(c => c.Id == q.Id)
                .Select(c => new
                {
                    c.Id,
                    c.Slug,
                    c.Level,
                    c.IsActive,
                    c.CreatedAtUtc,
                    c.PublishedAtUtc,
                    c.Price,
                    c.DurationHours,

                    CoverAssetId = _db.MediaAttachments
                        .Where(a => a.OwnerType == MediaOwnerType.Course &&
                                    a.OwnerKey == c.OwnerKey &&
                                    a.IsCover)
                        .Select(a => (long?)a.MediaAssetId)
                        .FirstOrDefault(),

                    Locales = c.Locales
                        .OrderBy(l => l.Culture)
                        .Select(l => new CourseLocaleDto(
                            l.Culture,
                            l.Title,
                            l.ShortDescription,
                            l.FullDescription))
                        .ToList(),

                    // ВСЕ нужные поля из MediaAsset
                    Media = _db.MediaAttachments
                        .Where(a => a.OwnerType == MediaOwnerType.Course &&
                                    a.OwnerKey == c.OwnerKey)
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
                        .ToList()
                })
                .FirstOrDefaultAsync(ct);

            if (row is null)
                throw new NotFoundException("course_not_found");

            // собираем VM в памяти + строим Url
            var media = row.Media
                .Select(m => new CourseMediaVm(
                    m.MediaAssetId,
                    MediaUrlHelper.ToUrl(_opt.Value,m.StoredPath),
                    ThumbUrl: m.ThumbStoredPath is null ? null : MediaUrlHelper.ToUrl(_opt.Value, m.ThumbStoredPath),
                    m.Type,                              
                    m.SortOrder,
                    m.IsCover,
                    m.CreatedAtUtc))
                .ToList();

            return new CourseAdminDto(
                Id: row.Id,
                Slug: row.Slug,
                Level: row.Level,
                IsActive: row.IsActive,
                CreatedAtUtc: row.CreatedAtUtc,
                PublishedAtUtc: row.PublishedAtUtc,
                Price: row.Price,
                DurationHours: row.DurationHours,
                CoverMediaId: row.CoverAssetId,
                Locales: row.Locales,
                Media: media
            );
        }
    }

}