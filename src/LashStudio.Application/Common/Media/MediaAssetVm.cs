namespace LashStudio.Application.Common.Media
{
    public sealed record MediaAssetVm(
     long Id,
     string Url,
     string? PosterUrl = null,
     string? Type = null // "image" | "video" (по желанию)
 );
}
