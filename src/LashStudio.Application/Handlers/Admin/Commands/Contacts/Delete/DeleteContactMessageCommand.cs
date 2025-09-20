using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Contacts.Delete
{
    public sealed record DeleteContactMessageCommand(long Id) : IRequest<Unit>;
}
