using LashStudio.Application.Common.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LashStudio.Infrastructure.Auth
{
    public sealed class JwtTokenService : IJwtProvider
    {
        private readonly JwtOptions _opt;
        private readonly TimeProvider _time;

        public JwtTokenService(IOptions<JwtOptions> opt, TimeProvider time)
        {
            _opt = opt.Value;
            _time = time;
        }

        public string CreateAccessToken(long userId, string email, IEnumerable<string> roles)
        {
            var now = _time.GetUtcNow().UtcDateTime;

            var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(ClaimTypes.NameIdentifier, userId.ToString()),
            new(ClaimTypes.Name, email)
        };
            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _opt.Issuer,
                audience: _opt.Audience,
                claims: claims,
                notBefore: now,
                expires: now.AddMinutes(_opt.AccessTokenMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}
