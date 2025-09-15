using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Reorder
{
    public sealed class ReorderCourseMediaHandler : IRequestHandler<ReorderCourseMediaCommand>
    {
        private readonly IAppDbContext _db;
        public ReorderCourseMediaHandler(IAppDbContext db) => _db = db;

        public async Task Handle(ReorderCourseMediaCommand cmd, CancellationToken ct)
        {
            var current = await _db.CourseMedia
                .Where(x => x.CourseId == cmd.CourseId)
                .OrderBy(x => x.SortOrder)
                .ToListAsync(ct);

            // Набор должен совпасть
            var currentIds = current.Select(x => x.MediaAssetId).OrderBy(x => x).ToArray();
            var newIds = cmd.AssetIdsInOrder.OrderBy(x => x).ToArray();

            if (currentIds.Length != newIds.Length || !currentIds.SequenceEqual(newIds))
            {
                var details = new Dictionary<string, string[]>
                {
                    ["assetIds"] = new[] { "media_set_mismatch" }
                };
                throw new AppValidationException("validation_failed", "validation_failed", details);
            }

            // Назначим новые порядки
            var rank = cmd.AssetIdsInOrder.Select((id, i) => (id, i))
                                          .ToDictionary(x => x.id, x => x.i);
            foreach (var row in current)
                row.SortOrder = rank[row.MediaAssetId];

            await _db.SaveChangesAsync(ct);
        }
    }
}
