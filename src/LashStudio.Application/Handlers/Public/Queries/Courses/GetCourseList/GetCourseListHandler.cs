using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Handlers.Admin.Commands.Courses.DTO;
using LashStudio.Application.Handlers.Public.Queries.Blog.GetBlogs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Public.Queries.Courses.GetCourseList
{
    public sealed class GetCourseListHandler
    : IRequestHandler<GetCourseListQuery, PagedResult<CourseListItemVm>>
    {
        private readonly IAppDbContext _db;
        private readonly IMediaUrlBuilder _media; // твой сервис построения URL
        public GetCourseListHandler(IAppDbContext db, IMediaUrlBuilder media) { _db = db; _media = media; }

        public async Task<PagedResult<CourseListItemVm>> Handle(GetCourseListQuery q, CancellationToken ct)
        {
            var baseQ = _db.Courses.AsNoTracking()
                .Where(x => x.IsActive)
                .Where(x => !q.Level.HasValue || x.Level == q.Level.Value);

            var total = await baseQ.CountAsync(ct);

            var items = await baseQ
                .OrderByDescending(x => x.PublishedAtUtc ?? x.CreatedAtUtc)
                .Skip((q.Page - 1) * q.PageSize)
                .Take(q.PageSize)
                .Select(x => new {
                    x.Slug,
                    x.Level,
                    x.Price,
                    x.DurationHours,
                    x.CoverMediaId,
                    Title = x.Locales.Where(l => l.Culture == q.Culture).Select(l => l.Title).FirstOrDefault()
                            ?? x.Locales.Select(l => l.Title).FirstOrDefault()
                })
                .ToListAsync(ct);

            var vms = items.Select(i => new CourseListItemVm(
                i.Slug, i.Title ?? "(no title)", i.Level, i.Price, i.DurationHours,
                i.CoverMediaId is null ? null : _media.Url(i.CoverMediaId.Value)
            )).ToList();

            return new PagedResult<CourseListItemVm>(total, q.Page, q.PageSize, vms);
        }
    }
}
