using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactProfileLocaleConfiguration : IEntityTypeConfiguration<ContactProfileLocale>
    {
        public void Configure(EntityTypeBuilder<ContactProfileLocale> e)
        {
            e.Property(x => x.Culture).HasMaxLength(10);
            e.HasIndex(x => new { x.ContactProfileId, x.Culture }).IsUnique();

            e.Property(x => x.OrganizationName).HasMaxLength(200);
            e.Property(x => x.AddressLine).HasMaxLength(500);
            e.Property(x => x.HowToFindUs).HasMaxLength(1000);
        }
    }
}
