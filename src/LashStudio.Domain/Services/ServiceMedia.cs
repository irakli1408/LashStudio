namespace LashStudio.Domain.Services
{
    public class ServiceMedia
    {
        public Guid Id { get; set; }
        public Guid ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        public Guid MediaAssetId { get; set; }   // ссылка на вашу таблицу/хранилище ассетов
        public int SortOrder { get; set; }
        public bool IsCover { get; set; }
        public Guid? PosterAssetId { get; set; } // постер для видео
    }
}
