using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactCtaLocaleConfiguration : IEntityTypeConfiguration<ContactCtaLocale>
    {
        public void Configure(EntityTypeBuilder<ContactCtaLocale> e)
        {
            e.Property(x => x.Culture).HasMaxLength(10).IsRequired();
            e.Property(x => x.Label).HasMaxLength(200).IsRequired();

            // В рамках одной CTA — по одной записи на культуру
            e.HasIndex(x => new { x.ContactCtaId, x.Culture }).IsUnique();
        }
    }
}
