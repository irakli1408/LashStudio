using LashStudio.Domain.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Services
{
    public class ServiceMediaConfiguration : IEntityTypeConfiguration<ServiceMedia>
    {
        public void Configure(EntityTypeBuilder<ServiceMedia> b)
        {
            b.ToTable("ServiceMedia");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.ServiceId, x.SortOrder });
        }
    }
}
