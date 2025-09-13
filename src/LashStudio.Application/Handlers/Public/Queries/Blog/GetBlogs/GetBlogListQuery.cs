using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs
{
    public record BlogListItemVm(
     int Id, string Slug, string Title,
     string? Excerpt, string? CoverUrl, DateTime? PublishedAt);

    public record PagedResult<T>(int Total, int Page, int PageSize, List<T> Items);

    public record GetBlogListQuery(string Culture, int Page = 1, int PageSize = 10)
        : IRequest<PagedResult<BlogListItemVm>>;
}
