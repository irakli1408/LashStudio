using LashStudio.Domain.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Media
{
    public sealed class MediaAssetConfig : IEntityTypeConfiguration<MediaAsset>
    {
        public void Configure(EntityTypeBuilder<MediaAsset> e)
        {
            e.ToTable("MediaAssets");
            e.HasKey(x => x.Id);

            e.Property(x => x.Type).IsRequired();

            e.Property(x => x.OriginalFileName)
                .HasMaxLength(255)
                .IsRequired();

            e.Property(x => x.StoredPath)
                .HasMaxLength(300)
                .IsRequired();

            e.Property(x => x.ContentType)
                .HasMaxLength(100)
                .IsRequired();

            e.Property(x => x.SizeBytes).IsRequired();

            // Метаданные
            e.Property(x => x.Width);
            e.Property(x => x.Height);
            e.Property(x => x.DurationSec);
            e.Property(x => x.PosterPath).HasMaxLength(300);

            // Время
            e.Property(x => x.CreatedAtUtc).HasDefaultValueSql("SYSUTCDATETIME()");

            // Новые поля
            e.Property(x => x.Extension).HasMaxLength(10);
            e.Property(x => x.HashSha256).HasMaxLength(64);

            // Soft delete
            e.Property(x => x.IsDeleted).HasDefaultValue(false);
            e.Property(x => x.DeletedAtUtc);

            // Индексы
            e.HasIndex(x => x.Type);
            e.HasIndex(x => x.CreatedAtUtc);
            e.HasIndex(x => x.IsDeleted);
            e.HasIndex(x => x.Extension);

            // По хэшу — если хочешь уникальность, можно включить:
            // e.HasIndex(x => x.HashSha256)
            //     .IsUnique()
            //     .HasFilter("[HashSha256] IS NOT NULL");

            // Фильтр — скрываем удалённые
            e.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}
