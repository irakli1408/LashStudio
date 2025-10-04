namespace LashStudio.Domain.Auth
{
    public class RefreshToken
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Token { get; set; } = default!;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime ExpiresAtUtc { get; set; }
        public DateTime? RevokedAtUtc { get; set; }
        public string? ReplacedByToken { get; set; }
        public string? Ip { get; set; }
        public string? UserAgent { get; set; }
    }
}
