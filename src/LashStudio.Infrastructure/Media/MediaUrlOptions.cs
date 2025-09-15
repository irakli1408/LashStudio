namespace LashStudio.Infrastructure.Media
{
    public sealed class MediaUrlOptions
    {
        public string BaseUrl { get; set; } = "";     // например, https://cdn.example.com
        public string PathFormat { get; set; } = "/asset/{id}"; // если нужен кастомный путь
    }
}
