namespace LashStudio.Domain.Services
{
    public class ServiceLocale
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public string Culture { get; set; } = default!;     // "ru", "en", "ka"...
        public string Title { get; set; } = default!;
        public string? ShortDescription { get; set; }
        public string? FullDescription { get; set; }
    }
}
