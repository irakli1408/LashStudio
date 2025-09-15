using LashStudio.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Courses
{
    public sealed class CourseMediaConfig : IEntityTypeConfiguration<CourseMedia>
    {
        public void Configure(EntityTypeBuilder<CourseMedia> b)
        {
            b.ToTable("CourseMedia");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.CourseId, x.MediaAssetId }).IsUnique();
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.HasOne(x => x.MediaAsset)
                .WithMany()
                .HasForeignKey(x => x.MediaAssetId)
                .OnDelete(DeleteBehavior.Restrict);

            b.HasOne(x => x.PosterAsset)
                .WithMany()
                .HasForeignKey(x => x.PosterAssetId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
