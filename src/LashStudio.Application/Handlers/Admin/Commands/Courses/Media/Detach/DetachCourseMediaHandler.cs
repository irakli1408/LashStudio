using LashStudio.Application.Common.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Detach
{
    public sealed class DetachCourseMediaHandler : IRequestHandler<DetachCourseMediaCommand>
    {
        private readonly IAppDbContext _db;
        public DetachCourseMediaHandler(IAppDbContext db) => _db = db;

        public async Task Handle(DetachCourseMediaCommand cmd, CancellationToken ct)
        {
            var row = await _db.CourseMedia
                .FirstOrDefaultAsync(x => x.CourseId == cmd.CourseId && x.MediaAssetId == cmd.AssetId, ct);

            if (row is null) return; // идемпотентно

            _db.CourseMedia.Remove(row);

            // если удаляем текущую обложку — снимем её
            var course = await _db.Courses.FirstOrDefaultAsync(x => x.Id == cmd.CourseId, ct);
            if (course is not null && course.CoverMediaId == cmd.AssetId)
                course.CoverMediaId = null;

            await _db.SaveChangesAsync(ct);
        }
    }
}
