using Microsoft.AspNetCore.Identity;
namespace LashStudio.Domain.Auth
{
    public class ApplicationUser : IdentityUser<long>
    {
        public string? DisplayName { get; set; }
        public bool IsBlocked { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
