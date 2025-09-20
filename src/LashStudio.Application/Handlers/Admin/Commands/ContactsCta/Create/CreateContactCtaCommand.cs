using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Create
{
    public sealed record CreateContactCtaCommand(ContactCtaCreateDto Dto) : IRequest<long>;
}
