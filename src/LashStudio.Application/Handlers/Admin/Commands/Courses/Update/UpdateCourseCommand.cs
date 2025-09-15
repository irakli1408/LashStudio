using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Domain.Courses;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Update
{
    public sealed record UpdateCourseCommand(
     long Id, string Slug, CourseLevel Level, decimal? Price, int? DurationHours,
     IReadOnlyList<CourseLocaleDto> Locales, long? CoverMediaId
 ) : IRequest;
}
