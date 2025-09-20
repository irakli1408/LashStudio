using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Delete
{
    public record DeleteServiceCommand(Guid Id) : IRequest;
}

