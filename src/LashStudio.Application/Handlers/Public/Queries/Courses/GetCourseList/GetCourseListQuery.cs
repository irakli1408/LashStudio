using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Courses;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseList
{
    public sealed record GetCourseListQuery(string Culture, int Page = 1, int PageSize = 12, CourseLevel? Level = null) : IRequest<PagedResult<CourseListItemVm>>;
}
