using LashStudio.Application.Common.Abstractions;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Logout
{
    public sealed class LogoutHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IRefreshTokenStore _store;
        public LogoutHandler(IRefreshTokenStore store) => _store = store;
        public async Task<Unit> Handle(LogoutCommand r, CancellationToken ct)
        {
            await _store.RevokeAsync(r.RefreshToken, "logout", ct);
            return Unit.Value;
        }
    }
}
