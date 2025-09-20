using LashStudio.Application.Contracts.Contacts;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.Contacts.GetContactProfileAdmin
{
    public sealed record GetContactProfileAdminQuery() : IRequest<ContactProfileAdminVm>;

}
