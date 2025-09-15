using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Detach
{
    public sealed record DetachCourseMediaCommand(long CourseId, long AssetId) : IRequest;
}
