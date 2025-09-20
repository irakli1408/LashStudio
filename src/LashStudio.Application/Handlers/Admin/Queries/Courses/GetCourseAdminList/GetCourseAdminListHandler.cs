using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Common.Queries.ListMedia;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace LashStudio.Application.Handlers.Admin.Queries.Courses.GetCourseAdminList
{
    public sealed class GetCourseAdminListHandler
     : IRequestHandler<GetCourseAdminListQuery, PagedResult<CourseAdminListItemVm>>
    {
        private readonly IAppDbContext _db;
        public GetCourseAdminListHandler(IAppDbContext db) => _db = db;

        public async Task<PagedResult<CourseAdminListItemVm>> Handle(GetCourseAdminListQuery q, CancellationToken ct)
        {
            var baseQ = _db.Courses.AsNoTracking();

            string? s = null;
            if (!string.IsNullOrWhiteSpace(q.Search))
            {
                s = q.Search.Trim().ToLower();

                baseQ = baseQ.Where(c =>
                    c.Slug.ToLower().Contains(s) ||
                    c.Locales.Any(l => (l.Title ?? "").ToLower().Contains(s)));
            }

            var total = await baseQ.CountAsync(ct);

           



            var items = await baseQ
                .OrderByDescending(x => x.PublishedAtUtc ?? x.CreatedAtUtc)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(c => new CourseAdminListItemVm(
                    c.Id,
                    c.Slug,
                    c.Level,
                    c.IsActive,
                    c.CreatedAtUtc,
                    c.PublishedAtUtc,
                    // <-- если есть поиск, берем заголовок, где он совпал; иначе первую локаль
                    (s != null
                        ? c.Locales.Where(l => ((l.Title ?? "").ToLower().Contains(s)))
                                   .Select(l => l.Title)
                                   .FirstOrDefault()
                        : null)
                    ?? c.Locales.Select(l => l.Title).FirstOrDefault(),
                    c.CoverMediaId))
                .ToListAsync(ct);



            return new PagedResult<CourseAdminListItemVm>(total, q.Page, q.PageSize, items);
        }
    }
}
