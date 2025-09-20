using LashStudio.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Media
{
    public sealed class MediaAttachmentConfig : IEntityTypeConfiguration<MediaAttachment>
    {
        public void Configure(EntityTypeBuilder<MediaAttachment> b)
        {
            b.ToTable("MediaAttachments");
            b.HasKey(x => x.Id);

            b.Property(x => x.OwnerType).HasConversion<short>();
            b.Property(x => x.OwnerKey).HasMaxLength(64).IsRequired();  // ← новое поле
            b.Property(x => x.SortOrder).HasDefaultValue(0);
            b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

            b.HasIndex(x => new { x.OwnerType, x.OwnerKey, x.MediaAssetId }).IsUnique();
            b.HasIndex(x => new { x.OwnerType, x.OwnerKey }).IsUnique().HasFilter("[IsCover] = 1");
        }
    }
}
