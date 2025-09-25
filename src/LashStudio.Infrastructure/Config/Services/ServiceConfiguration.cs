using LashStudio.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Services
{
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> b)
        {
            b.ToTable("Services");
            b.HasKey(x => x.Id);

            b.Property(x => x.Slug).IsRequired().HasMaxLength(200);
            b.HasIndex(x => x.Slug).IsUnique();

            b.Property(x => x.Price).HasColumnType("decimal(10,2)");
            b.Property(x => x.Category).HasConversion<short>();
            b.Property(x => x.Variant).HasConversion<short?>();

            b.Property(x => x.OwnerKey)
                .HasColumnType("varchar(36)")
                .HasComputedColumnSql("LOWER(CONVERT(varchar(36), [Id]))", stored: true);

            // Индекс для быстрых коррелированных подзапросов
            b.HasIndex(x => x.OwnerKey).HasDatabaseName("IX_Services_OwnerKey");

            b.HasMany(x => x.Locales)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
