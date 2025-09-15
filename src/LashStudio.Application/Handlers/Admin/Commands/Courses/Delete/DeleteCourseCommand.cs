using MediatR;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Delete
{
    public sealed record DeleteCourseCommand(long Id) : IRequest;

}
