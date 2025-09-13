using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogBySlug
{
    public record BlogPostVm(
    int Id, string Slug, string Title, string Content,
    string? CoverUrl, DateTime? PublishedAt);

    public record GetBlogBySlugQuery(string Culture, string Slug) : IRequest<BlogPostVm>;
}
