using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Refresh
{
    public sealed record RefreshCommand(string RefreshToken) : IRequest<AuthResponse>;
}
