using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Domain.Courses;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Create
{
    public sealed record CreateCourseCommand(
    string Slug, CourseLevel Level, decimal? Price, int? DurationHours,
    IReadOnlyList<CourseLocaleDto> Locales, long? CoverMediaId = null
) : IRequest<long>;
}
