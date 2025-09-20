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

            b.HasMany(x => x.Locales)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            b.HasMany(x => x.Media)
                .WithOne(x => x.Service)
                .HasForeignKey(x => x.ServiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
