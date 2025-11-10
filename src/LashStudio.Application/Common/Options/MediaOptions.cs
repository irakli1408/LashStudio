namespace LashStudio.Application.Common.Options;

public class MediaOptions
{
    public string RootPath { get; set; } = "uploads";
    public string RequestPath { get; set; } = "/media";
    public int MaxImageSizeMb { get; set; } = 20;
    public int MaxVideoSizeMb { get; set; } = 2000;
    public string[] AllowedImageExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    public string[] AllowedVideoExtensions { get; set; } = new[] { ".mp4", ".webm" };

    public int ThumbMaxWidth { get; set; } = 320; // или 480
    public int JpegQualityMain { get; set; } = 85;
    public int JpegQualityThumb { get; set; } = 70;
}
