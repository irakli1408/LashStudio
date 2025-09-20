namespace LashStudio.Domain.Contacts
{
    public class ContactCta
    {
        public long Id { get; set; }

        public long ContactProfileId { get; set; }
        public ContactProfile ContactProfile { get; set; } = default!;

        public CtaKind Kind { get; set; } = CtaKind.Instagram;

        /// <summary>Порядок отображения CTA (меньше — выше).</summary>
        public int Order { get; set; } = 0;

        /// <summary>Включена ли кнопка.</summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Полный URL, если хочешь задать вручную (перебивает автогенерацию).
        /// Например: https://ig.me/m/yourname
        /// </summary>
        public string? UrlOverride { get; set; }

        public ICollection<ContactCtaLocale> Locales { get; set; } = new List<ContactCtaLocale>();
    }
}
