using LashStudio.Application.Contracts.AboutPerson;
using MediatR;

namespace LashStudio.Application.Handlers.Public.Queries.AboutPerson
{
    public sealed record GetAboutPublicQuery(string? Culture) : IRequest<AboutPublicVm>;
}
