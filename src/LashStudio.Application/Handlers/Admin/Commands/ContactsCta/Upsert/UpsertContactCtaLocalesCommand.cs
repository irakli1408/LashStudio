using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Upsert
{
    public sealed record UpsertContactCtaLocalesCommand(long CtaId, IReadOnlyList<ContactCtaLocaleUpsertDto> Locales) : IRequest<Unit>;
}
