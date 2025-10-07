using LashStudio.Application.Contracts.Faq;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public sealed record UpdateFaqItemCommand(FaqItemAdminVm Body) : IRequest<FaqItemAdminVm>;
}
