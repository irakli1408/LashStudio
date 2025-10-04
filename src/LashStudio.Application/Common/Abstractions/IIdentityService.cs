using LashStudio.Application.Contracts;

namespace LashStudio.Application.Common.Abstractions
{
    public interface IIdentityService
    {
        Task<UserInfo?> FindByEmailAsync(string email, CancellationToken ct);
        Task<(bool Success, UserInfo? User)> CheckPasswordAsync(string email, string password, bool lockoutOnFailure, CancellationToken ct);
        Task<UserInfo> CreateUserAsync(string email, string password, string? displayName, CancellationToken ct);
        Task<UserInfo?> FindByIdAsync(long userId, CancellationToken ct);
        Task<string[]> GetRolesAsync(long userId, CancellationToken ct);
        Task AddToRoleAsync(long userId, string role, CancellationToken ct);
        Task RemoveFromRoleAsync(long userId, string role, CancellationToken ct);
    }
}
