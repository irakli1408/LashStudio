using LashStudio.Application.Contracts.AboutPerson;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Queries.AboutPerson
{
    public sealed record GetAboutAdminQuery() : IRequest<AboutAdminDto>;

}
