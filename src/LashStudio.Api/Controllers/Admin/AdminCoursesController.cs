using Asp.Versioning;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Create;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Update;
using LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminList;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/admin/courses")]
    public sealed class AdminCoursesController : ControllerBase
    {
        private readonly IMediator _m;
        public AdminCoursesController(IMediator m) => _m = m;

        [HttpGet]
        [OutputCache(PolicyName = "public-60s")]
        public Task<PagedResult<CourseAdminListItemVm>> List([FromQuery] GetCourseAdminListQuery q, CancellationToken ct)
       => _m.Send(q, ct);

        [HttpGet("{id:long}")]
        [OutputCache(PolicyName = "public-60s")]
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
