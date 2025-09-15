using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Reorder
{
    public sealed record ReorderCourseMediaCommand(long CourseId, IReadOnlyList<long> AssetIdsInOrder) : IRequest;
}
