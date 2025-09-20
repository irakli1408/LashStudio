namespace LashStudio.Domain.Contacts
{
    public enum CtaKind
    {
        None = 0,
        Instagram = 1,
        Telegram = 2,
        WhatsApp = 3,
        Phone = 4,
        Email = 5
    }

    public enum ContactMessageStatus
    {
        New = 0,
        Read = 1,
        Replied = 2,
        Archived = 9
    }

    public class ContactProfile
    {
        public long Id { get; set; }

        // Контакты (не локализуются)
        public string? EmailPrimary { get; set; }
        public string? EmailSales { get; set; }
        public string[] Phones { get; set; } = [];

        // Соцсети / мессенджеры (храним username/номер, без протоколов)
        public string? Instagram { get; set; }  // username без @
        public string? Telegram { get; set; }   // username без @
        public string? WhatsApp { get; set; }   // номер в международном формате, без + (для wa.me)

        // Карта
        public decimal? MapLat { get; set; }
        public decimal? MapLng { get; set; }
        public int MapZoom { get; set; } = 15;

        // Предпочтительная CTA (чтобы Instagram был приоритетным по умолчанию)
        public CtaKind PreferredCta { get; set; } = CtaKind.Instagram;

        // Время работы (ОПЦИОНАЛЬНО — коллекция может быть пустой)
        public ICollection<ContactBusinessHour> Hours { get; set; } = new List<ContactBusinessHour>();

        // Локали
        public ICollection<ContactProfileLocale> Locales { get; set; } = new List<ContactProfileLocale>();

        public ICollection<ContactCta> Ctas { get; set; } = new List<ContactCta>();


        // SEO
        public string? SeoTitle { get; set; }
        public string? SeoDescription { get; set; }
    }
}
