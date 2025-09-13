using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Posts
{
    public record PostLocaleAdminVm(int Id, string Culture, string Title, string Slug, string Content);

    public record PostAdminVm(
        int Id,
        string Status,
        DateTime CreatedAt,
        DateTime? PublishedAt,
        long? CoverMediaId,
        string? CoverUrl,
        List<PostLocaleAdminVm> Locales);

    public record GetPostByIdQuery(int Id) : IRequest<PostAdminVm>;
}
