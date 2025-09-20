using LashStudio.Application.Contracts.AboutPerson;
using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
{
    public sealed record UpsertAboutPageCommand(AboutUpsertCommandDto Model) : IRequest<long>;
}
