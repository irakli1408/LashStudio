using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Delete
{
    public sealed record DeleteAboutPageCommand() : IRequest<Unit>;
}
