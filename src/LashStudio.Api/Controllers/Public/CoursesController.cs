using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseDetails;
using LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseList;
using LashStudio.Domain.Courses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v1/{culture}/courses")]
public sealed class CoursesController : ControllerBase
{
    private readonly IMediator _m;
    public CoursesController(IMediator m) => _m = m;

    [HttpGet]
    [OutputCache(PolicyName = "public-10m-tagged")]
    public Task<PagedResult<CourseListItemVm>> List(string culture, [FromQuery] int page = 1, [FromQuery] int pageSize = 12, [FromQuery] CourseLevel? level = null, CancellationToken ct = default)
        => _m.Send(new GetCourseListQuery(culture, page, pageSize, level), ct);

    [HttpGet("{slug}")]
    [OutputCache(PolicyName = "public-10m-tagged")]
    public Task<CourseDetailsVm> Details(string culture, string slug, CancellationToken ct)
        => _m.Send(new GetCourseDetailsQuery(culture, slug), ct);
}
