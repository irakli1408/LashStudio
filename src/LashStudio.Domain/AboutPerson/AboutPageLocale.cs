namespace LashStudio.Domain.AboutPerson
{
    public sealed class AboutPageLocale
    {
        public long Id { get; set; }                         // identity
        public long AboutPageId { get; set; }
        public AboutPage AboutPage { get; set; } = default!;

        public string Culture { get; set; } = default!;      // "ka-GE", "ru-RU", "en-US"
        public string Title { get; set; } = default!;
        public string? SubTitle { get; set; }
        public string BodyHtml { get; set; } = default!;
    }
}
