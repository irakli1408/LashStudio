using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Contacts.Upsert
{
    public sealed record UpsertContactProfileCommand(ContactProfileUpsertDto Dto) : IRequest<Unit>;
}
