using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.SetCourse
{
    public sealed class SetCourseCoverHandler : IRequestHandler<SetCourseCoverCommand>
    {
        private readonly IAppDbContext _db;
        public SetCourseCoverHandler(IAppDbContext db) => _db = db;

        public async Task Handle(SetCourseCoverCommand cmd, CancellationToken ct)
        {
            var course = await _db.Courses.FirstOrDefaultAsync(x => x.Id == cmd.CourseId, ct)
                ?? throw new NotFoundException("course_not_found", "course_not_found");

            var attached = await _db.CourseMedia.AnyAsync(
                x => x.CourseId == cmd.CourseId && x.MediaAssetId == cmd.AssetId, ct);

            if (!attached)
                throw new BadRequestException("asset_not_attached", "asset_not_attached");

            course.CoverMediaId = cmd.AssetId;

            await _db.SaveChangesAsync(ct);
        }
    }
}
