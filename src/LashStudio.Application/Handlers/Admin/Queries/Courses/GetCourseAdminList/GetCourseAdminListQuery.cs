using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminList
{
    public sealed record GetCourseAdminListQuery(int Page = 1, int PageSize = 20, string? Search = null)
    : IRequest<PagedResult<CourseAdminListItemVm>>;
}
