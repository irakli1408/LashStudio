using LashStudio.Domain.Abstractions;
using LashStudio.Domain.Abtraction;
using LashStudio.Domain.Media;

namespace LashStudio.Domain.Courses
{
    public enum CourseLevel : short
    {
        Unknown = 0,
        Basic = 1,  // базовый
        Advanced = 2,  // продвинутый
        Pro = 3   // профи/эксперт
    }

    public sealed class Course : IHasId<long>, IActivatable, ISortable, ICreatedAtUtc
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = false;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public string Slug { get; set; } = default!;
        public CourseLevel Level { get; set; }
        public decimal? Price { get; set; }
        public int? DurationHours { get; set; }
        public DateTime? PublishedAtUtc { get; set; }

        // Обложка (быстрый доступ как в Post)
        public long? CoverMediaId { get; set; }
        public MediaAsset? CoverMedia { get; set; }

        // Галерея
        public List<CourseMedia> Media { get; set; } = new();

        // Локали
        public List<CourseLocale> Locales { get; set; } = new();
    }
}
