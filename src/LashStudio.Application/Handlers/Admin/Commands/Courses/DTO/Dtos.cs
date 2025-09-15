using LashStudio.Application.Common.Media;
using LashStudio.Domain.Courses;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.DTO
{
    public sealed record CourseLocaleDto(string Culture, string Title, string? ShortDescription, string? FullDescription);

    public sealed record CourseAdminListItemVm(
        long Id, string Slug, CourseLevel Level, bool IsActive,
        DateTime CreatedAtUtc, DateTime? PublishedAtUtc, string? TitleAny, long? CoverMediaId);

    public sealed record CourseAdminDto(
        long Id, string Slug, CourseLevel Level, bool IsActive, DateTime CreatedAtUtc, DateTime? PublishedAtUtc,
        decimal? Price, int? DurationHours, long? CoverMediaId,
        IReadOnlyList<CourseLocaleDto> Locales,
        IReadOnlyList<CourseMediaVm> Media);

    public sealed record CourseMediaVm(long MediaAssetId, int SortOrder, bool IsCover, long? PosterAssetId);

    // Публичные модели
    public sealed record CourseListItemVm(
        string Slug, string Title, CourseLevel Level, decimal? Price, int? DurationHours, string? CoverUrl);

    public sealed record CourseDetailsVm(
        string Slug, string Title, string? ShortDescription, string? FullDescription,
        CourseLevel Level, decimal? Price, int? DurationHours,
        string? CoverUrl,
        IReadOnlyList<MediaAssetVm> Gallery);
}
