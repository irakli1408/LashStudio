using LashStudio.Domain.Abstractions;

namespace LashStudio.Domain.Faq
{
    public class FaqItem : IHasId<long>, IActivatable
    {
        public long Id { get; set; }
        public bool IsActive { get; set; } = true;
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        public List<FaqItemLocale> Locales { get; set; } = new();
    }
}