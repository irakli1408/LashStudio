using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Publish.Post
{
    public record PublishPostCommand(int PostId, bool Publish) : IRequest;
}
