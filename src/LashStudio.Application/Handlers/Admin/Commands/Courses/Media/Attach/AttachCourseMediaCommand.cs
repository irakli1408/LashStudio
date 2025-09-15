using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.Attach
{
    public sealed record AttachCourseMediaCommand(long CourseId, long AssetId) : IRequest;
}
