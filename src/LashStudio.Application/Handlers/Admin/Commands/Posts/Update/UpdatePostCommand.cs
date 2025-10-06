using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Update
{
    public record UpdatePostCommand(
        int Id,
        List<PostLocaleInput> Locales,
        long? CoverMediaId
    ) : IRequest;

}