using Asp.Versioning;
using LashStudio.Api.Contracts.Posts;
using LashStudio.Application.Handlers.Admin.Commands.Posts;
using LashStudio.Application.Handlers.Admin.Commands.Posts.Create;
using LashStudio.Application.Handlers.Admin.Commands.Posts.Delete;
using LashStudio.Application.Handlers.Admin.Commands.Posts.Update;
using LashStudio.Application.Handlers.Admin.Queries.Posts;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Admin
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/admin/posts")]
    public sealed class AdminPostsController : ApiControllerBase
    {
        public AdminPostsController(ISender sender) : base(sender) { }

        [HttpGet("{id:int}")]
        public Task<PostAdminVm> GetById(int id)
         => Sender.Send(new GetPostByIdQuery(id));

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreatePostDto dto)
        {
            var id = await Sender.Send(new CreatePostCommand(
                dto.Locales.Select(l => new PostLocaleInput(l.Culture, l.Title, l.Slug, l.Content)).ToList(),
                dto.CoverMediaId));

            return Created($"/api/v1/{{culture}}/admin/posts/{id}", new { id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdatePostDto dto)
        {
            var locales = dto.Locales.Select(l => new PostLocaleInput(l.Culture, l.Title, l.Slug, l.Content)).ToList();
            await Sender.Send(new UpdatePostCommand(id, locales, dto.CoverMediaId));
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await Sender.Send(new DeletePostCommand(id));
            return NoContent();
        }
    }
}
