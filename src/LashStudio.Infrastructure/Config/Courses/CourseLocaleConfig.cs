using LashStudio.Domain.Courses;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LashStudio.Infrastructure.Config.Courses
{
    public sealed class CourseLocaleConfig : IEntityTypeConfiguration<CourseLocale>
    {
        public void Configure(EntityTypeBuilder<CourseLocale> b)
        {
            b.ToTable("CourseLocales");
            b.HasKey(x => x.Id);

            b.HasIndex(x => new { x.CourseId, x.Culture }).IsUnique();
            b.Property(x => x.Culture).HasMaxLength(8).IsRequired();
            b.Property(x => x.Title).HasMaxLength(512).IsRequired();
        }
    }
}
