using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
    {
        public void Configure(EntityTypeBuilder<ContactMessage> e)
        {
            e.HasIndex(x => new { x.Status, x.CreatedAtUtc });

            e.Property(x => x.Status).HasConversion<int>();

            e.Property(x => x.Name).HasMaxLength(200);
            e.Property(x => x.Phone).HasMaxLength(40);
            e.Property(x => x.Email).HasMaxLength(200);
            e.Property(x => x.Subject).HasMaxLength(300);

            e.Property(x => x.Body).IsRequired();
            e.Property(x => x.ClientIp).HasMaxLength(64);
        }
    }
}
