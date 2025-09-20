using LashStudio.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Services
{
    public class ServiceLocaleConfiguration : IEntityTypeConfiguration<ServiceLocale>
    {
        public void Configure(EntityTypeBuilder<ServiceLocale> b)
        {
            b.ToTable("ServiceLocales");
            b.HasKey(x => x.Id);

            b.Property(x => x.Culture).IsRequired().HasMaxLength(10);
            b.Property(x => x.Title).IsRequired().HasMaxLength(300);

            b.HasIndex(x => new { x.ServiceId, x.Culture }).IsUnique();
        }
    }
}
