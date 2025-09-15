using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Media;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails
{
    public sealed class GetCourseDetailsHandler
    : IRequestHandler<GetCourseDetailsQuery, CourseDetailsVm>
    {
        private readonly IAppDbContext _db;
        private readonly IMediaUrlBuilder _media;
        public GetCourseDetailsHandler(IAppDbContext db, IMediaUrlBuilder media) { _db = db; _media = media; }

        public async Task<CourseDetailsVm> Handle(GetCourseDetailsQuery q, CancellationToken ct)
        {
            var e = await _db.Courses.AsNoTracking()
                .Include(x => x.Locales)
                .Include(x => x.Media)
                .FirstOrDefaultAsync(x => x.IsActive && x.Slug == q.Slug, ct)
                ?? throw new NotFoundException("course_not_found", "course_not_found");

            var loc = e.Locales.FirstOrDefault(l => l.Culture == q.Culture) ?? e.Locales.FirstOrDefault();

            var coverUrl = e.CoverMediaId is null ? null : _media.Url(e.CoverMediaId.Value);

            var gallery = e.Media.OrderBy(m => m.SortOrder)
                .Select(m => new MediaAssetVm(
                    m.MediaAssetId,
                    _media.Url(m.MediaAssetId),
                    m.PosterAssetId.HasValue ? _media.Url(m.PosterAssetId.Value) : null))
                .ToList();

            return new CourseDetailsVm(
                e.Slug,
                loc?.Title ?? "(no title)",
                loc?.ShortDescription,
                loc?.FullDescription,
                e.Level, e.Price, e.DurationHours,
                coverUrl,
                gallery
            );
        }
    }
}
