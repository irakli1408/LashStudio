using LashStudio.Domain.Abstractions;
using LashStudio.Domain.Abtraction;

namespace LashStudio.Domain.Services
{
    public enum ServiceCategory : short
    {
        LashExtension = 1,
        LashLamination = 2,
        BrowLamination = 3
    }

    public enum LashExtensionVariant : short
    {
        OneD = 1, TwoD = 2, ThreeD = 3, FourD = 4, MegaVolume = 5
    }

    public class Service : IHasId<Guid>, IActivatable, ICreatedAtUtc
    {
        public Guid Id { get; set; }
        public string Slug { get; set; } = default!;
        public ServiceCategory Category { get; set; }
        public LashExtensionVariant? Variant { get; set; } // только для LashExtension
        public decimal Price { get; set; }
        public int? DurationMinutes { get; set; }

        public Guid? CoverMediaId { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? PublishedAtUtc { get; set; }
        public string OwnerKey { get; private set; } = default!;

        public ICollection<ServiceLocale> Locales { get; set; } = new List<ServiceLocale>();
    }
}
