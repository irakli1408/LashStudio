using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Update
{
    public sealed class UpdateCourseHandler : IRequestHandler<UpdateCourseCommand>
    {
        private readonly IAppDbContext _db;
        public UpdateCourseHandler(IAppDbContext db) => _db = db;

        public async Task Handle(UpdateCourseCommand c, CancellationToken ct)
        {
            var e = await _db.Courses.Include(x => x.Locales)
                .FirstOrDefaultAsync(x => x.Id == c.Id, ct)
                ?? throw new NotFoundException("course_not_found", "course_not_found");

            var newSlug = c.Slug.Trim().ToLowerInvariant();
            if (!newSlug.Equals(e.Slug, StringComparison.OrdinalIgnoreCase))
            {
                var taken = await _db.Courses.AnyAsync(x => x.Slug == newSlug && x.Id != e.Id, ct);
                if (taken) throw new ConflictException("slug_taken", "slug_taken");
                e.Slug = newSlug;
            }

            e.Level = c.Level;
            e.Price = c.Price;
            e.DurationHours = c.DurationHours;
            e.CoverMediaId = c.CoverMediaId;

            // простая перезапись локалей
            _db.CourseLocales.RemoveRange(e.Locales);
            e.Locales = c.Locales.Select(l => new CourseLocale
            {
                Culture = l.Culture.Trim().ToLowerInvariant(),
                Title = l.Title.Trim(),
                ShortDescription = l.ShortDescription,
                FullDescription = l.FullDescription
            }).ToList();

            await _db.SaveChangesAsync(ct);
        }
    }
}
