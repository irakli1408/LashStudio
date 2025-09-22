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
            b.Property(x => x.OwnerKey).HasMaxLength(64).IsRequired();
            b.Property(x => x.SortOrder).HasDefaultValue(0);
            b.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");
            b.Property(x => x.IsCover).HasDefaultValue(false); // если поле есть

            // FK → MediaAsset (важно: Restrict, и навигация на коллекцию Attachments)
            b.HasOne(x => x.MediaAsset)
             .WithMany(a => a.Attachments)
             .HasForeignKey(x => x.MediaAssetId)
             .OnDelete(DeleteBehavior.Restrict);

            // уникальные ограничения
            b.HasIndex(x => new { x.OwnerType, x.OwnerKey, x.MediaAssetId }).IsUnique();

            // один cover на Owner
            b.HasIndex(x => new { x.OwnerType, x.OwnerKey })
             .IsUnique()
             .HasFilter("[IsCover] = 1");
        }
    }
}
