using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Delete
{
    public sealed class DeleteContactCtaCommandValidator : AbstractValidator<DeleteContactCtaCommand>
    {
        public DeleteContactCtaCommandValidator()
        {           
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
