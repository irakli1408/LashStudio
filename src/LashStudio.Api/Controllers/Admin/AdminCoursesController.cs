using LashStudio.Application.Handlers.Admin.Commands.Courses.Create;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Update;
using LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminList;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [Route("api/admin/courses")]
    public sealed class AdminCoursesController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminCoursesController(IMediator m) => _m = m;

        [HttpGet]
        public Task<PagedResult<CourseAdminListItemVm>> List([FromQuery] GetCourseAdminListQuery q, CancellationToken ct)
       => _m.Send(q, ct);

        [HttpGet("{id:long}")]
        public Task<CourseAdminDto> Details(long id, CancellationToken ct)
            => _m.Send(new GetCourseAdminDetailsQuery(id), ct);

        [HttpPost]
        public Task<long> Create([FromBody] CreateCourseCommand cmd, CancellationToken ct)
            => _m.Send(cmd, ct);

        [HttpPut("{id:long}")]
        public Task Update(long id, [FromBody] UpdateCourseCommand cmd, CancellationToken ct)
            => _m.Send(cmd with { Id = id }, ct);

        [HttpDelete("{id:long}")]
        public Task Delete(long id, CancellationToken ct)
            => _m.Send(new DeleteCourseCommand(id), ct);
    }
}
