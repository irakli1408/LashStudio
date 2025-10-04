using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.MakeAdmin
{
    public sealed class MakeAdminHandler : IRequestHandler<MakeAdminCommand, Unit>
    {
        private readonly IIdentityService _ids;
        public MakeAdminHandler(IIdentityService ids) => _ids = ids;

        public async Task<Unit> Handle(MakeAdminCommand c, CancellationToken ct)
        {
            await _ids.AddToRoleAsync(c.UserId, "Admin", ct);
            return Unit.Value;
        }
    }
}
