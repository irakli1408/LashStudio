using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

public record GetCourseAdminDetailsQuery(long Id) : IRequest<CourseAdminDto>;

public sealed class GetCourseAdminDetailsHandler : IRequestHandler<GetCourseAdminDetailsQuery, CourseAdminDto>
{
    private readonly IAppDbContext _db;
    private readonly IMediaUrlBuilder _media;

    public GetCourseAdminDetailsHandler(IAppDbContext db, IMediaUrlBuilder media)
    {
        _db = db;
        _media = media;
    }

    public async Task<CourseAdminDto> Handle(GetCourseAdminDetailsQuery q, CancellationToken ct)
    {
        // 1) Один поход в БД: курс + coverId + "сырые" медиа
        var row = await _db.Courses.AsNoTracking()
            .Where(c => c.Id == q.Id)
            .Select(c => new
            {
                c.Id,
                c.Slug,
                c.Level,
                c.IsActive,
                c.CreatedAtUtc,
                c.PublishedAtUtc,
                c.Price,
                c.DurationHours,

                // cover (IsCover приоритетно)
                CoverAssetId = _db.MediaAttachments
                    .Where(a => a.OwnerType == MediaOwnerType.Course && a.OwnerKey == c.OwnerKey && a.IsCover)
                    .Select(a => (long?)a.MediaAssetId)
                    .FirstOrDefault(),

                // все локали
                Locales = c.Locales
                    .OrderBy(l => l.Culture)
                    .Select(l => new CourseLocaleDto(l.Culture, l.Title, l.ShortDescription, l.FullDescription))
                    .ToList(),

                // вся галерея (без Url — его соберём в памяти)
                Media = _db.MediaAttachments
                    .Where(a => a.OwnerType == MediaOwnerType.Course && a.OwnerKey == c.OwnerKey)
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new { a.MediaAssetId, a.SortOrder, a.IsCover })
                    .ToList()
            })
            .FirstOrDefaultAsync(ct);

        if (row is null)
            throw new NotFoundException("course_not_found");

        // 2) Сборка итоговой VM в памяти (Url строим здесь; это не новый поход в БД)
        var media = row.Media
            .Select(m => new CourseMediaVm(
                m.MediaAssetId,
                _media.Url(m.MediaAssetId),
                m.SortOrder,
                m.IsCover))
            .ToList();

        return new CourseAdminDto(
            Id: row.Id,
            Slug: row.Slug,
            Level: row.Level,
            IsActive: row.IsActive,
            CreatedAtUtc: row.CreatedAtUtc,
            PublishedAtUtc: row.PublishedAtUtc,
            Price: row.Price,
            DurationHours: row.DurationHours,
            CoverMediaId: row.CoverAssetId,
            Locales: row.Locales,
            Media: media
        );
    }
}
