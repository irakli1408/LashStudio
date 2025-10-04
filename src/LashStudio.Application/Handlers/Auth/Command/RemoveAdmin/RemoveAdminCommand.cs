using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.RemoveAdmin
{
    public sealed record RemoveAdminCommand(long UserId) : MediatR.IRequest<Unit>;
}
