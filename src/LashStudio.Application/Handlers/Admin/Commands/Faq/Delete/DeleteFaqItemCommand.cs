using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Delete
{
    public record DeleteFaqItemCommand(long Id) : IRequest;
}
