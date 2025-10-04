using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Refresh
{
    public sealed class RefreshHandler : IRequestHandler<RefreshCommand, AuthResponse>
    {
        private readonly IRefreshTokenStore _store;
        private readonly IIdentityService _ids;
        private readonly IJwtProvider _jwt;
        private readonly TimeProvider _time;

        public RefreshHandler(IRefreshTokenStore store, IIdentityService ids, IJwtProvider jwt, TimeProvider time)
        { _store = store; _ids = ids; _jwt = jwt; _time = time; }

        public async Task<AuthResponse> Handle(RefreshCommand r, CancellationToken ct)
        {
            var (valid, userId) = await _store.ValidateAsync(r.RefreshToken, ct);
            if (!valid) throw new UnauthorizedAccessException("refresh_invalid");

            var u = await _ids.FindByIdAsync(userId, ct) ?? throw new UnauthorizedAccessException();
            var roles = await _ids.GetRolesAsync(u.Id, ct);

            var newRefresh = await _store.RotateAsync(
                r.RefreshToken,
                _time.GetUtcNow().UtcDateTime.AddDays(14),
                null, null, ct);

            var newAccess = _jwt.CreateAccessToken(u.Id, u.Email, roles);
            return new AuthResponse(newAccess, newRefresh, u.Id, u.Email, roles);
        }
    }
}
