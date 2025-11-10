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

    // ► Новые поля для превью (thumbnail)
    public string? ThumbStoredPath { get; set; }     // yyyy/MM/000_guid_thumb.ext
    public int? ThumbWidth { get; set; }
    public int? ThumbHeight { get; set; }

    public string? Extension { get; set; }           // ".jpg" / ".mp4" (нормализовано)
    public string? HashSha256 { get; set; }

    // soft delete
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAtUtc { get; set; }

    public ICollection<MediaAttachment> Attachments { get; set; } = new List<MediaAttachment>();
}
