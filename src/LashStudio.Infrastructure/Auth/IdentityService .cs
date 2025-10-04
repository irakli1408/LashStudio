using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Contracts;
using LashStudio.Domain.Auth;
using Microsoft.AspNetCore.Identity;

namespace LashStudio.Infrastructure.Auth
{
    public sealed class IdentityService : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _users;
        private readonly SignInManager<ApplicationUser> _signIn;

        public IdentityService(UserManager<ApplicationUser> users, SignInManager<ApplicationUser> signIn)
        { _users = users; _signIn = signIn; }

        public async Task<LashStudio.Application.Contracts.UserInfo?> FindByEmailAsync(string email, CancellationToken ct)
        {
            var u = await _users.FindByEmailAsync(email);
            return u is null ? null : new LashStudio.Application.Contracts.UserInfo(u.Id, u.Email!, u.DisplayName);
        }

        public async Task<(bool Success, LashStudio.Application.Contracts.UserInfo? User)> CheckPasswordAsync(string email, string password, bool lockoutOnFailure, CancellationToken ct)
        {
            var u = await _users.FindByEmailAsync(email);
            if (u is null) return (false, null);
            var res = await _signIn.CheckPasswordSignInAsync(u, password, lockoutOnFailure);
            return (res.Succeeded, res.Succeeded ? new LashStudio.Application.Contracts.UserInfo(u.Id, u.Email!, u.DisplayName) : null);
        }

        public async Task<LashStudio.Application.Contracts.UserInfo> CreateUserAsync(string email, string password, string? displayName, CancellationToken ct)
        {
            var u = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, DisplayName = displayName };
            var res = await _users.CreateAsync(u, password);
            if (!res.Succeeded) throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
            return new LashStudio.Application.Contracts.UserInfo(u.Id, u.Email!, u.DisplayName);
        }

        public async Task<string[]> GetRolesAsync(long userId, CancellationToken ct)
        {
            var u = await _users.FindByIdAsync(userId.ToString()) ?? throw new KeyNotFoundException("user_not_found");
            var roles = await _users.GetRolesAsync(u);
            return roles.ToArray();
        }

        public async Task AddToRoleAsync(long userId, string role, CancellationToken ct)
        {
            var u = await _users.FindByIdAsync(userId.ToString()) ?? throw new KeyNotFoundException("user_not_found");
            var res = await _users.AddToRoleAsync(u, role);
            if (!res.Succeeded) throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        public async Task RemoveFromRoleAsync(long userId, string role, CancellationToken ct)
        {
            var u = await _users.FindByIdAsync(userId.ToString()) ?? throw new KeyNotFoundException("user_not_found");
            var res = await _users.RemoveFromRoleAsync(u, role);
            if (!res.Succeeded) throw new InvalidOperationException(string.Join("; ", res.Errors.Select(e => e.Description)));
        }

        public async Task<LashStudio.Application.Contracts.UserInfo?> FindByIdAsync(long userId, CancellationToken ct)
        {
            var u = await _users.FindByIdAsync(userId.ToString());
            return u is null ? null : new UserInfo(u.Id, u.Email!, u.DisplayName);
        }
    }
}
