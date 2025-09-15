using LashStudio.Domain.Abstractions;

namespace LashStudio.Domain.Courses
{
    public sealed class CourseLocale : IHasId<long>
    {
        public long Id { get; set; }
        public long CourseId { get; set; }
        public string Culture { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }

        public Course Course { get; set; } = default!;
    }

}
