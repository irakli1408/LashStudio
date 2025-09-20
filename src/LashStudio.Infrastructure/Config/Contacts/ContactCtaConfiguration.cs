using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactCtaConfiguration : IEntityTypeConfiguration<ContactCta>
    {
        public void Configure(EntityTypeBuilder<ContactCta> e)
        {
            // enum -> int
            e.Property(x => x.Kind).HasConversion<int>();

            e.Property(x => x.UrlOverride).HasMaxLength(500);

            // В одном профиле можно иметь несколько CTA, сортируем по Order
            e.HasIndex(x => new { x.ContactProfileId, x.Order });

            // Связи
            e.HasOne(x => x.ContactProfile)
                .WithMany(p => p.Ctas)
                .HasForeignKey(x => x.ContactProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Locales)
                .WithOne(l => l.ContactCta)
                .HasForeignKey(l => l.ContactCtaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
