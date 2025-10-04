using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Register
{
    public sealed record RegisterCommand(string Email, string Password, string? DisplayName) : IRequest<AuthResponse>;
}
