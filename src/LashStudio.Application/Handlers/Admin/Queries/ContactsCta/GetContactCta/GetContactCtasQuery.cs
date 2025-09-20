using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.ContactsCta.GetContactCta
{
    public sealed record GetContactCtasQuery() : IRequest<IReadOnlyList<ContactCtaAdminVm>>;
}
