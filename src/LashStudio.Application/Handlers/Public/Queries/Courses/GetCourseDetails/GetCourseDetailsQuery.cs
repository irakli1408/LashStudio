using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails
{
    public sealed record GetCourseDetailsQuery(string Culture, string Slug) : IRequest<CourseDetailsVm>;
}
