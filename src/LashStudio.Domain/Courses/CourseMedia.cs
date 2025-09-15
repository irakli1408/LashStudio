using LashStudio.Domain.Abstractions;
using LashStudio.Domain.Abtraction;
using LashStudio.Domain.Media;

namespace LashStudio.Domain.Courses
{
    // связь с множеством ассетов + порядок + опциональный постер для видео
    public sealed class CourseMedia : IHasId<long>, ISortable
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public long MediaAssetId { get; set; }
        public int SortOrder { get; set; } = 0;

        public bool IsCover { get; set; } = false;   // если хочешь, чтобы обложка была из галереи
        public long? PosterAssetId { get; set; }     // постер к видео (не обязателен)

        public Course Course { get; set; } = default!;
        public MediaAsset MediaAsset { get; set; } = default!;
        public MediaAsset? PosterAsset { get; set; }
    }
}
