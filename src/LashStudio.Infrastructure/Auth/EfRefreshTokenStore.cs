using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Auth;
using LashStudio.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LashStudio.Infrastructure.Auth
{
    public sealed class EfRefreshTokenStore : IRefreshTokenStore
    {
        private readonly AppDbContext _db;
        private readonly TimeProvider _time;
        public EfRefreshTokenStore(AppDbContext db, TimeProvider time) { _db = db; _time = time; }

        public async Task<string> IssueAsync(long userId, DateTime expiresAt, string? ip, string? agent, CancellationToken ct)
        {
            var token = Generate();
            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                Token = token,
                ExpiresAtUtc = expiresAt,
                Ip = ip,
                UserAgent = agent
            });
            await _db.SaveChangesAsync(ct);
            return token;
        }

        public async Task<(bool Valid, long UserId)> ValidateAsync(string token, CancellationToken ct)
        {
            var now = _time.GetUtcNow().UtcDateTime;
            var rt = await _db.RefreshTokens.AsNoTracking()
                .FirstOrDefaultAsync(x => x.Token == token, ct);
            if (rt is null || rt.RevokedAtUtc != null || rt.ExpiresAtUtc <= now)
                return (false, 0);
            return (true, rt.UserId);
        }

        public async Task<string> RotateAsync(string oldToken, DateTime newExpiresAt, string? ip, string? agent, CancellationToken ct)
        {
            var now = _time.GetUtcNow().UtcDateTime;
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == oldToken, ct)
                     ?? throw new KeyNotFoundException("refresh_not_found");

            if (rt.RevokedAtUtc != null || rt.ExpiresAtUtc <= now)
                throw new InvalidOperationException("refresh_invalid");

            var newToken = Generate();
            rt.RevokedAtUtc = now;
            rt.ReplacedByToken = newToken;

            _db.RefreshTokens.Add(new RefreshToken
            {
                UserId = rt.UserId,
                Token = newToken,
                ExpiresAtUtc = newExpiresAt,
                Ip = ip,
                UserAgent = agent
            });

            await _db.SaveChangesAsync(ct);
            return newToken;
        }

        public async Task RevokeAsync(string token, string reason, CancellationToken ct)
        {
            var rt = await _db.RefreshTokens.FirstOrDefaultAsync(x => x.Token == token, ct);
            if (rt is null) return;
            rt.RevokedAtUtc = _time.GetUtcNow().UtcDateTime;
            await _db.SaveChangesAsync(ct);
        }

        private static string Generate()
        {
            Span<byte> buf = stackalloc byte[32];
            RandomNumberGenerator.Fill(buf);
            return Convert.ToBase64String(buf);
        }
    }
}
