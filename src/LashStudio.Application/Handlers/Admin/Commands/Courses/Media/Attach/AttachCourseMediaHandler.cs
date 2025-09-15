using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using LashStudio.Domain.Courses;
using LashStudio.Domain.Media;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Attach
{
    public sealed class AttachCourseMediaHandler : IRequestHandler<AttachCourseMediaCommand>
    {
        private readonly IAppDbContext _db;
        public AttachCourseMediaHandler(IAppDbContext db) => _db = db;

        public async Task Handle(AttachCourseMediaCommand cmd, CancellationToken ct)
        {
            // Проверим, что курс есть
            var existsCourse = await _db.Courses.AnyAsync(x => x.Id == cmd.CourseId, ct);
            if (!existsCourse) throw new NotFoundException("course_not_found", "course_not_found");

            // Проверим ассет и тип
            var asset = await _db.MediaAssets.FirstOrDefaultAsync(x => x.Id == cmd.AssetId, ct)
                ?? throw new NotFoundException("media_asset_not_found", "media_asset_not_found");

            if (!IsAllowed(asset.Type))
                throw new UnsupportedMediaTypeAppException("unsupported_media_type", "unsupported_media_type");

            // Идемпотентность
            var already = await _db.CourseMedia.AnyAsync(
                x => x.CourseId == cmd.CourseId && x.MediaAssetId == cmd.AssetId, ct);
            if (already) return;

            // В конец списка
            var maxOrder = await _db.CourseMedia
                .Where(x => x.CourseId == cmd.CourseId)
                .Select(x => (int?)x.SortOrder)
                .MaxAsync(ct) ?? -1;

            _db.CourseMedia.Add(new CourseMedia
            {
                CourseId = cmd.CourseId,
                MediaAssetId = cmd.AssetId,
                SortOrder = maxOrder + 1
            });

            await _db.SaveChangesAsync(ct);
        }

        private static bool IsAllowed(MediaType t) => t == MediaType.Photo || t == MediaType.Video;
    }
}
