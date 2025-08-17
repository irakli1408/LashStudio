namespace LashStudio.Domain.Media;

public class MediaAsset
{
    public long Id { get; set; }
    public MediaType Type { get; set; }

    public string OriginalFileName { get; set; } = "";
    public string StoredPath { get; set; } = "";     // yyyy/MM/guid.ext
    public string ContentType { get; set; } = "";
    public long SizeBytes { get; set; }

    // опциональные метаданные
    public int? Width { get; set; }
    public int? Height { get; set; }
    public double? DurationSec { get; set; }
    public string? PosterPath { get; set; }          // для видео

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
