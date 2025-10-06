using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Delete
{
    public sealed class DeleteCourseCommandValidator : AbstractValidator<DeleteCourseCommand>
    {
        public DeleteCourseCommandValidator()
        {           
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
