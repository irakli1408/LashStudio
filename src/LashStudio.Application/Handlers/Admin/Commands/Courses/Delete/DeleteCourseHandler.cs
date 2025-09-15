using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Delete
{
    public sealed class DeleteCourseHandler : IRequestHandler<DeleteCourseCommand>
    {
        private readonly IAppDbContext _db;
        public DeleteCourseHandler(IAppDbContext db) => _db = db;

        public async Task Handle(DeleteCourseCommand c, CancellationToken ct)
        {
            var e = await _db.Courses.FirstOrDefaultAsync(x => x.Id == c.Id, ct)
                ?? throw new NotFoundException("course_not_found", "course_not_found");

            _db.Courses.Remove(e); // каскадом удалятся Locales/Media
            await _db.SaveChangesAsync(ct);
        }
    }

}
