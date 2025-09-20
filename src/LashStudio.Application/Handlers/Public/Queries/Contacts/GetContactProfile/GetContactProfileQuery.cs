using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile
{
    public sealed record GetContactProfileQuery(string? Culture = null)
     : IRequest<ContactProfileVm>;
}
