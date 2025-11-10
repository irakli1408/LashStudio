using LashStudio.Domain.Abstractions;

namespace LashStudio.Domain.AboutPerson
{
    public sealed class AboutPage : IActivatable, IHasId<long>
    {
        public long Id { get; set; }                         // long IDENTITY
        public bool IsActive { get; set; }
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAtUtc { get; set; }

        public ICollection<AboutPageLocale> Locales { get; set; } = new List<AboutPageLocale>();
    }
}
