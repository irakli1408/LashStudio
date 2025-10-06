using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Delete
{
    public sealed class DeleteServiceCommandValidator : AbstractValidator<DeleteServiceCommand>
    {
        public DeleteServiceCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id must be a non-empty GUID.");
        }
    }
}
