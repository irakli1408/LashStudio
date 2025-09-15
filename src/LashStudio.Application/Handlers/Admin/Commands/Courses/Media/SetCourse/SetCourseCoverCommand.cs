using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Media.SetCourse
{
    public sealed record SetCourseCoverCommand(long CourseId, long AssetId) : IRequest;
}
