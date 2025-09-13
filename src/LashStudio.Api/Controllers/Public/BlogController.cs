using Asp.Versioning;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogBySlug;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LashStudio.Api.Controllers.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/{culture}/blog")]
    public sealed class BlogController : ApiControllerBase
    {
        public BlogController(ISender sender) : base(sender) { }

        [HttpGet]
        public Task<PagedResult<BlogListItemVm>> List(
            string culture, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
            => Sender.Send(new GetBlogListQuery(culture, page, pageSize));

        [HttpGet("{slug}")]
        public Task<BlogPostVm> GetBySlug(string culture, string slug)
            => Sender.Send(new GetBlogBySlugQuery(culture, slug));
    }
}
