using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminDetails
{
    public sealed class GetCourseAdminDetailsHandler
     : IRequestHandler<GetCourseAdminDetailsQuery, CourseAdminDto>
    {
        private readonly IAppDbContext _db;
        public GetCourseAdminDetailsHandler(IAppDbContext db) => _db = db;

        public async Task<CourseAdminDto> Handle(GetCourseAdminDetailsQuery q, CancellationToken ct)
        {
            var e = await _db.Courses.AsNoTracking()
         .Include(x => x.Locales)
         .Include(x => x.Media)
         .FirstOrDefaultAsync(x => x.Id == q.Id, ct)
         ?? throw new NotFoundException("course_not_found", "course_not_found");

            return new CourseAdminDto(
                e.Id, e.Slug, e.Level, e.IsActive, e.CreatedAtUtc, e.PublishedAtUtc,
                e.Price, e.DurationHours, e.CoverMediaId,
                e.Locales.Select(l => new CourseLocaleDto(l.Culture, l.Title, l.ShortDescription, l.FullDescription)).ToList(),
                e.Media.OrderBy(m => m.SortOrder)
                       .Select(m => new CourseMediaVm(
                           m.MediaAssetId,
                           m.SortOrder,
                           e.CoverMediaId == m.MediaAssetId,   
                           m.PosterAssetId))
                       .ToList()
                    );
        }
    }
}
