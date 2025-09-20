using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Delete
{
    public sealed record DeleteContactCtaCommand(long Id) : IRequest<Unit>;
}
