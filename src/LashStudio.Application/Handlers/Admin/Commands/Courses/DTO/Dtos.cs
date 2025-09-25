using LashStudio.Domain.Courses;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.DTO
{
    public sealed record CourseLocaleDto(string Culture, string Title, string? ShortDescription, string? FullDescription);

    public sealed record CourseMediaVm(
     long AssetId,
     string Url,
     int SortOrder,
     bool IsCover);

    public sealed record CourseAdminListItemVm(
        long Id,
        string Slug,
        CourseLevel Level,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? PublishedAtUtc,
        string? TitleAny,
        long? CoverMediaId,
        IReadOnlyList<CourseMediaVm> Media);

    public sealed record CourseAdminDto(
        long Id,
        string Slug,
        CourseLevel Level,
        bool IsActive,
        DateTime CreatedAtUtc,
        DateTime? PublishedAtUtc,
        decimal? Price,
        int? DurationHours,
        long? CoverMediaId,
        IReadOnlyList<CourseLocaleDto> Locales,
        IReadOnlyList<CourseMediaVm> Media);


    // Публичные модели
    public sealed record CourseListItemVm(
     string Slug,
     string Title,
     CourseLevel Level,
     decimal? Price,
     int? DurationHours,
     string? CoverUrl,
     List<CourseMediaVm> Media);

    public sealed record CourseDetailsVm(
    long Id,                       // ← добавили
    string Slug,
    string Title,
    string? ShortDescription,
    string? FullDescription,
    CourseLevel Level,
    decimal? Price,
    int? DurationHours,
    string? CoverUrl,
    IReadOnlyList<CourseMediaVm> Media);
}