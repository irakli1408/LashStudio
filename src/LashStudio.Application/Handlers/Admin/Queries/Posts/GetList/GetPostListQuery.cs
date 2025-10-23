using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Posts.GetList
{
    public sealed record GetAdminPostListQuery(string Culture)
    : GetAdminPostListQueryParams, IRequest<PagedResult<AdminPostListItemVm>>;

    public sealed record AdminPostListItemVm(
        int Id,
        bool IsActive,
        string Status,
        DateTime CreatedAt,
        DateTime? PublishedAt,
        long? CoverMediaId,
        string? CoverUrl,
        string Title,
        string Slug
    );

    public record GetAdminPostListQueryParams
    {
        public bool? IsPublished { get; init; }
        public bool IsActive { get; init; }
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? Search { get; init; }
        public string? Sort { get; init; } = "createdAt_desc";
    }
}
