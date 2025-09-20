using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactBusinessHourConfiguration : IEntityTypeConfiguration<ContactBusinessHour>
    {
        public void Configure(EntityTypeBuilder<ContactBusinessHour> e)
        {
            // уникальность дня в рамках профиля
            e.HasIndex(x => new { x.ContactProfileId, x.Day }).IsUnique();

            // TimeOnly маппится в time (если провайдер поддерживает; у EF Core 7/8 — да)
            e.Property(x => x.Open);
            e.Property(x => x.Close);
        }
    }
}
