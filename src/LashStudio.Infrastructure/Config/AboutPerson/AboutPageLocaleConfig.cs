using LashStudio.Domain.AboutPerson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.AboutPerson
{
    public sealed class AboutPageLocaleConfig : IEntityTypeConfiguration<AboutPageLocale>
    {
        public void Configure(EntityTypeBuilder<AboutPageLocale> b)
        {
            b.ToTable("AboutPageLocales");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).ValueGeneratedOnAdd();

            b.Property(x => x.Culture).IsRequired().HasMaxLength(10);
            b.Property(x => x.Title).IsRequired().HasMaxLength(256);
            b.Property(x => x.SubTitle).HasMaxLength(256);
            b.Property(x => x.BodyHtml).IsRequired();

            // уникальность локали для страницы
            b.HasIndex(x => new { x.AboutPageId, x.Culture }).IsUnique();
        }
    }
}
