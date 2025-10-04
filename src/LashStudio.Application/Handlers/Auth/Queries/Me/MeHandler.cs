using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Auth.Queries.MeResponses;
using MediatR;

namespace LashStudio.Application.Handlers.Auth.Queries.Me
{
    public sealed class MeHandler : IRequestHandler<MeQuery, MeResponse>
    {
        private readonly IIdentityService _ids;
        public MeHandler(IIdentityService ids) => _ids = ids;

        public async Task<MeResponse> Handle(MeQuery q, CancellationToken ct)
        {
            var u = await _ids.FindByIdAsync(q.UserId, ct) ?? throw new UnauthorizedAccessException();
            var roles = await _ids.GetRolesAsync(u.Id, ct);
            return new MeResponse(u.Id, u.Email, u.DisplayName, roles);
        }
    }
}
