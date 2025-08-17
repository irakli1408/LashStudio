namespace LashStudio.Application.Common.Options;

public class MediaOptions
{
    public string RootPath { get; set; } = "uploads";
    public string RequestPath { get; set; } = "/media";
    public int MaxImageSizeMb { get; set; } = 20;
    public int MaxVideoSizeMb { get; set; } = 2000;
    public string[] AllowedImageExtensions { get; set; } = new[] { ".jpg", ".jpeg", ".png", ".webp" };
    public string[] AllowedVideoExtensions { get; set; } = new[] { ".mp4", ".webm" };
}
