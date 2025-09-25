using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Media;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails
{
    public sealed class GetCourseDetailsHandler
     : IRequestHandler<GetCourseDetailsQuery, CourseDetailsVm>
    {
        private readonly IAppDbContext _db;
        private readonly IMediaUrlBuilder _media;

        public GetCourseDetailsHandler(IAppDbContext db, IMediaUrlBuilder media)
        { _db = db; _media = media; }

        public async Task<CourseDetailsVm> Handle(GetCourseDetailsQuery q, CancellationToken ct)
        {
            var e = await _db.Courses.AsNoTracking()
                .Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.IsActive && x.Slug == q.Slug, ct)
                ?? throw new NotFoundException("course_not_found", "course_not_found");

            var loc = e.Locales.FirstOrDefault(l => l.Culture == q.Culture) ?? e.Locales.FirstOrDefault();

            // батчить не нужно — один курс
            var ownerKey = e.Id.ToString(CultureInfo.InvariantCulture);

            var attachments = await _db.MediaAttachments.AsNoTracking()
                .Where(a => a.OwnerType == MediaOwnerType.Course && a.OwnerKey == ownerKey)
                .OrderBy(a => a.SortOrder)
                .Select(a => new { a.MediaAssetId, a.SortOrder, a.IsCover })
                .ToListAsync(ct);

            var media = attachments
                .Select(a => new CourseMediaVm(
                    AssetId: a.MediaAssetId,
                    Url: _media.Url(a.MediaAssetId),
                    SortOrder: a.SortOrder,
                    IsCover: a.IsCover))
                .ToList();

            var coverUrl = e.CoverMediaId is not null
                ? _media.Url(e.CoverMediaId.Value)
                : media.FirstOrDefault(m => m.IsCover)?.Url;

            return new CourseDetailsVm(
                Id: e.Id,
                Slug: e.Slug,
                Title: loc?.Title ?? "(no title)",
                ShortDescription: loc?.ShortDescription,
                FullDescription: loc?.FullDescription,
                Level: e.Level,
                Price: e.Price,
                DurationHours: e.DurationHours,
                CoverUrl: coverUrl,
                Media: media
            );
        }
    }
}