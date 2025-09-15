using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Courses;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Create
{
    public sealed class CreateCourseHandler : IRequestHandler<CreateCourseCommand, long>
    {
        private readonly IAppDbContext _db;
        public CreateCourseHandler(IAppDbContext db) => _db = db;

        public async Task<long> Handle(CreateCourseCommand c, CancellationToken ct)
        {
            var slug = c.Slug.Trim().ToLowerInvariant();
            var taken = await _db.Courses.AnyAsync(x => x.Slug == slug, ct);
            if (taken) throw new ConflictException("slug_taken", "slug_taken");

            if (c.Locales == null || c.Locales.Count == 0)
                throw new AppValidationException(errors: new Dictionary<string, string[]> { ["Locales"] = new[] { "required" } });

            var e = new Course
            {
                Slug = slug,
                Level = c.Level,
                Price = c.Price,
                DurationHours = c.DurationHours,
                CoverMediaId = c.CoverMediaId,
                IsActive = false,
                CreatedAtUtc = DateTime.UtcNow
            };

            foreach (var loc in c.Locales)
            {
                if (string.IsNullOrWhiteSpace(loc.Title))
                    throw new AppValidationException(errors: (IReadOnlyDictionary<string, string[]>)new Dictionary<string, string[]> { ["Locales.Title"] = new[] { "required" } });

                e.Locales.Add(new CourseLocale
                {
                    Culture = loc.Culture.Trim().ToLowerInvariant(),
                    Title = loc.Title.Trim(),
                    ShortDescription = loc.ShortDescription,
                    FullDescription = loc.FullDescription
                });
            }

            _db.Courses.Add(e);
            await _db.SaveChangesAsync(ct);
            return e.Id;
        }
    }
}
