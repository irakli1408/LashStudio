using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.RemoveAdmin
{
    public sealed class RemoveAdminHandler : IRequestHandler<RemoveAdminCommand, Unit>
    {
        private readonly IIdentityService _ids;
        public RemoveAdminHandler(IIdentityService ids) => _ids = ids;

        public async Task<Unit> Handle(RemoveAdminCommand c, CancellationToken ct)
        {
            await _ids.RemoveFromRoleAsync(c.UserId, "Admin", ct);
            return Unit.Value;
        }
    }
}
