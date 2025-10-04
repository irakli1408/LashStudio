using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.MakeAdmin
{
    public sealed record MakeAdminCommand(long UserId) : MediatR.IRequest<Unit>;
}
