using LashStudio.Application.Handlers.Admin.Commands.Courses.Create;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Attach;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Detach;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Reorder;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Media.SetCourse;
using LashStudio.Application.Handlers.Admin.Commands.Courses.Update;
using LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminDetails;
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

        //// attach один ассет из твоего MediaAsset-флоу
        //[HttpPost("{id:long}/media/{assetId:long}")]
        //public Task Attach(long id, long assetId, CancellationToken ct)
        //    => _m.Send(new AttachCourseMediaCommand(id, assetId), ct);

        //[HttpDelete("{id:long}/media/{assetId:long}")]
        //public Task Detach(long id, long assetId, CancellationToken ct)
        //    => _m.Send(new DetachCourseMediaCommand(id, assetId), ct);

        //[HttpPost("{id:long}/media/reorder")]
        //public Task Reorder(long id, [FromBody] IReadOnlyList<long> assetIds, CancellationToken ct)
        //    => _m.Send(new ReorderCourseMediaCommand(id, assetIds), ct);

        //// назначить обложку (можно держать и IsCover в CourseMedia, и CoverMediaId в Course)
        //[HttpPost("{id:long}/media/{assetId:long}/cover")]
        //public Task SetCover(long id, long assetId, CancellationToken ct)
        //    => _m.Send(new SetCourseCoverCommand(id, assetId), ct);

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
