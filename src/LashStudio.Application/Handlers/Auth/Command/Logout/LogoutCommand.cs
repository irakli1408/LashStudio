using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Logout
{
    public sealed record LogoutCommand(string RefreshToken) : IRequest<Unit>;
}
