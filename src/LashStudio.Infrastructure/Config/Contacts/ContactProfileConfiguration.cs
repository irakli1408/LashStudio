using LashStudio.Domain.Contacts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Contacts
{
    public sealed class ContactProfileConfiguration : IEntityTypeConfiguration<ContactProfile>
    {
        public void Configure(EntityTypeBuilder<ContactProfile> e)
        {
            // Phones: string[] <-> string
            e.Property(x => x.Phones)
                .HasConversion(
                    v => string.Join(";", v ?? Array.Empty<string>()),
                    v => string.IsNullOrWhiteSpace(v)
                            ? Array.Empty<string>()
                            : v.Split(';', StringSplitOptions.RemoveEmptyEntries));

            // enum -> int
            e.Property(x => x.PreferredCta)
                .HasConversion<int>();

            // длины/ограничения (по желанию)
            e.Property(x => x.EmailPrimary).HasMaxLength(200);
            e.Property(x => x.EmailSales).HasMaxLength(200);
            e.Property(x => x.Instagram).HasMaxLength(100);
            e.Property(x => x.Telegram).HasMaxLength(100);
            e.Property(x => x.WhatsApp).HasMaxLength(30);

            // отношения
            e.HasMany(x => x.Locales)
                .WithOne(x => x.ContactProfile)
                .HasForeignKey(x => x.ContactProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Hours)
                .WithOne(x => x.ContactProfile)
                .HasForeignKey(x => x.ContactProfileId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasMany(x => x.Ctas)
             .WithOne(x => x.ContactProfile)
             .HasForeignKey(x => x.ContactProfileId)
             .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
