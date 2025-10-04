namespace LashStudio.Application.Common.Abstractions
{
    public interface IRefreshTokenStore
    {
        Task<string> IssueAsync(long userId, DateTime expiresAt, string? ip, string? agent, CancellationToken ct);
        Task<(bool Valid, long UserId)> ValidateAsync(string token, CancellationToken ct);
        Task<string> RotateAsync(string oldToken, DateTime newExpiresAt, string? ip, string? agent, CancellationToken ct);
        Task RevokeAsync(string token, string reason, CancellationToken ct);
    }
}
