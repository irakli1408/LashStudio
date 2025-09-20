using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Update
{
    public sealed record UpdateContactCtaCommand(ContactCtaUpdateDto Dto) : IRequest<Unit>;
}
