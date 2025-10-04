using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Auth.Queries.AuthResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Command.Register
{
    public sealed class RegisterHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IIdentityService _ids; private readonly IJwtProvider _jwt; private readonly IRefreshTokenStore _store; private readonly TimeProvider _time;
        public RegisterHandler(IIdentityService ids, IJwtProvider jwt, IRefreshTokenStore store, TimeProvider time) { _ids = ids; _jwt = jwt; _store = store; _time = time; }

        public async Task<AuthResponse> Handle(RegisterCommand r, CancellationToken ct)
        {
            if (await _ids.FindByEmailAsync(r.Email, ct) is not null) throw new InvalidOperationException("email_in_use");
            var u = await _ids.CreateUserAsync(r.Email, r.Password, r.DisplayName, ct);
            await _ids.AddToRoleAsync(u.Id, "User", ct);
            var roles = await _ids.GetRolesAsync(u.Id, ct);

            var access = _jwt.CreateAccessToken(u.Id, u.Email, roles);
            var refresh = await _store.IssueAsync(u.Id, _time.GetUtcNow().UtcDateTime.AddDays(14), null, null, ct);
            return new AuthResponse(access, refresh, u.Id, u.Email, roles);
        }
    }
}

