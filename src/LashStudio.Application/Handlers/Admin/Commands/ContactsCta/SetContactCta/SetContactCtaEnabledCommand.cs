using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.SetContactCta
{
    public sealed record SetContactCtaEnabledCommand(long Id, bool IsEnabled) : IRequest<Unit>;
}
