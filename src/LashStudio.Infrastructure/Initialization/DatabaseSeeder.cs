using LashStudio.Domain.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LashStudio.Infrastructure.Initialization
{
    public static class DatabaseSeeder
    {
        /// <summary>
        /// Идемпотентный сид: создаёт роли и первого супер-админа.
        /// Требует, чтобы миграции БД были уже применены вручную.
        /// </summary>
        public static async Task SeedAsync(
            IServiceProvider services,
            IConfiguration cfg,
            IHostEnvironment env)
        {
            using var scope = services.CreateScope();
            var roleMgr = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<long>>>();
            var userMgr = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 1) Роли
            foreach (var name in new[] { "SuperAdmin", "Admin", "User" })
            {
                if (!await roleMgr.RoleExistsAsync(name))
                    _ = await roleMgr.CreateAsync(new IdentityRole<long>(name));
            }

            // 2) Супер-админ из конфигурации
            var email = cfg["SuperAdmin:Email"];
            var pwd = cfg["SuperAdmin:Password"];

            if (!string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(pwd))
            {
                var user = await userMgr.FindByEmailAsync(email);
                if (user is null)
                {
                    user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        EmailConfirmed = true,
                        DisplayName = "Super Admin"
                    };

                    var createRes = await userMgr.CreateAsync(user, pwd);
                    if (!createRes.Succeeded)
                        throw new Exception("Failed to create SuperAdmin: " +
                            string.Join("; ", createRes.Errors.Select(e => e.Description)));
                }

                if (!await userMgr.IsInRoleAsync(user, "SuperAdmin"))
                {
                    var addRoleRes = await userMgr.AddToRoleAsync(user, "SuperAdmin");
                    if (!addRoleRes.Succeeded)
                        throw new Exception("Failed to add SuperAdmin role: " +
                            string.Join("; ", addRoleRes.Errors.Select(e => e.Description)));
                }
            }

            // 3) (Опционально) демо-аккаунты только в Dev
            if (env.IsDevelopment())
            {
                await EnsureUserWithRole(userMgr, "admin@lashstudio.local", "Admin!12345", "Admin");
                await EnsureUserWithRole(userMgr, "user@lashstudio.local", "User!12345", "User");
            }
        }

        private static async Task EnsureUserWithRole(
            UserManager<ApplicationUser> userMgr,
            string email, string password, string role)
        {
            var u = await userMgr.FindByEmailAsync(email);
            if (u is null)
            {
                u = new ApplicationUser { UserName = email, Email = email, EmailConfirmed = true, DisplayName = email };
                var res = await userMgr.CreateAsync(u, password);
                if (!res.Succeeded)
                    throw new Exception($"Failed to create {role}: " +
                        string.Join("; ", res.Errors.Select(e => e.Description)));
            }
            if (!await userMgr.IsInRoleAsync(u, role))
                _ = await userMgr.AddToRoleAsync(u, role);
        }
    }
}
