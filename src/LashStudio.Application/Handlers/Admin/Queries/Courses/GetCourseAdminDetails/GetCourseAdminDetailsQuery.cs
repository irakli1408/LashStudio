using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminDetails
{
    public sealed record GetCourseAdminDetailsQuery(long Id) : IRequest<CourseAdminDto>;

}
