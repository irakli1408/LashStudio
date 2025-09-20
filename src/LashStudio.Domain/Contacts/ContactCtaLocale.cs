namespace LashStudio.Domain.Contacts
{
    public class ContactCtaLocale
    {
        public long Id { get; set; }

        public long ContactCtaId { get; set; }
        public ContactCta ContactCta { get; set; } = default!;

        public string Culture { get; set; } = "ru-RU";

        /// <summary>Подпись кнопки, напр. «Записаться в Instagram».</summary>
        public string Label { get; set; } = default!;
    }
}
