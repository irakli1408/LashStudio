using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Delete
{
    public record DeletePostCommand(int Id) : IRequest;
}
