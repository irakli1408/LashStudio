using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Create
{
    public record CreatePostCommand(
        List<PostLocaleInput> Locales,
        long? CoverMediaId = null
    ) : IRequest<long>;
}
