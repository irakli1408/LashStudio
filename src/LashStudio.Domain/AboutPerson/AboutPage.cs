using LashStudio.Domain.Abstractions;

namespace LashStudio.Domain.AboutPerson
{
    public sealed class AboutPage : IActivatable, IHasId<long>
    {
        public long Id { get; set; }                         // long IDENTITY
        public bool IsActive { get; set; }
        public bool IsCover { get; set; }                    // << НОВОЕ: раздел использует обложку (берём из MediaAttachment.IsCover)
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAtUtc { get; set; }

        // SEO
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
        public string? SeoKeywordsCsv { get; set; }

        public ICollection<AboutPageLocale> Locales { get; set; } = new List<AboutPageLocale>();
    }
}
