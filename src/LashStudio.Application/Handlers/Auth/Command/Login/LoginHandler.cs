using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Login
{
    public sealed class LoginHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IIdentityService _ids; private readonly IJwtProvider _jwt; private readonly IRefreshTokenStore _store; private readonly TimeProvider _time;
        public LoginHandler(IIdentityService ids, IJwtProvider jwt, IRefreshTokenStore store, TimeProvider time) { _ids = ids; _jwt = jwt; _store = store; _time = time; }

        public async Task<AuthResponse> Handle(LoginCommand r, CancellationToken ct)
        {
            var (ok, u) = await _ids.CheckPasswordAsync(r.Email, r.Password, true, ct);
            if (!ok || u is null) throw new UnauthorizedAccessException("invalid_credentials");
            var roles = await _ids.GetRolesAsync(u.Value.Id, ct);

            var access = _jwt.CreateAccessToken(u.Value.Id, u.Value.Email, roles);
            var refresh = await _store.IssueAsync(u.Value.Id, _time.GetUtcNow().UtcDateTime.AddDays(14), null, null, ct);
            return new AuthResponse(access, refresh, u.Value.Id, u.Value.Email, roles);
        }
    }
}
