using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Login
{
    public sealed record LoginCommand(string Email, string Password) : IRequest<AuthResponse>;
}
