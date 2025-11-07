using LashStudio.Domain.AboutPerson;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.AboutPerson
{
    public sealed class AboutPageConfig : IEntityTypeConfiguration<AboutPage>
    {
        public void Configure(EntityTypeBuilder<AboutPage> b)
        {
            b.ToTable("AboutPages");
            b.HasKey(x => x.Id);

            b.Property(x => x.Id).ValueGeneratedOnAdd();     // BIGINT IDENTITY(1,1)

            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.CreatedAtUtc).IsRequired();

            b.Property(x => x.SeoTitle).HasMaxLength(256);
            b.Property(x => x.SeoDescription).HasMaxLength(512);
            b.Property(x => x.SeoKeywordsCsv).HasMaxLength(512);

            b.HasMany(x => x.Locales)
             .WithOne(x => x.AboutPage)
             .HasForeignKey(x => x.AboutPageId)
             .OnDelete(DeleteBehavior.Cascade);

            // полезные индексы
            b.HasIndex(x => x.IsActive);
            b.HasIndex(x => x.PublishedAtUtc);
        }
    }
}
