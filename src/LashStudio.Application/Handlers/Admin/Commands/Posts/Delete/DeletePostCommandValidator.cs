using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Delete
{
    public sealed class DeletePostCommandValidator : AbstractValidator<DeletePostCommand>
    {
        public DeletePostCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
