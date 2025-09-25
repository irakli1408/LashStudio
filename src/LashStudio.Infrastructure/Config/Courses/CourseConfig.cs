using LashStudio.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Courses
{
    public sealed class CourseConfig : IEntityTypeConfiguration<Course>
    {
        public void Configure(EntityTypeBuilder<Course> b)
        {
            b.ToTable("Courses");
            b.HasKey(x => x.Id);

            b.HasIndex(x => x.Slug).IsUnique();
            b.Property(x => x.Slug).HasMaxLength(256).IsRequired();
            b.Property(x => x.Level).HasConversion<short>().IsRequired();
            b.Property(x => x.IsActive).IsRequired();
            b.Property(x => x.SortOrder).HasDefaultValue(0);

            b.HasOne(x => x.CoverMedia)
                .WithMany()
                .HasForeignKey(x => x.CoverMediaId)
                .OnDelete(DeleteBehavior.Restrict); // ассет не удаляем каскадно

            b.HasMany(x => x.Locales)
                .WithOne(l => l.Course)
                .HasForeignKey(l => l.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            b.Property(x => x.OwnerKey)
                 .HasColumnType("varchar(36)")
                 .HasComputedColumnSql("LOWER(CONVERT(varchar(36), [Id]))", stored: true);

            b.HasIndex(x => x.OwnerKey).HasDatabaseName("IX_Courses_OwnerKey");
        }
    }
}
