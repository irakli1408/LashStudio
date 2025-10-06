using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Delete
{
    public sealed class DeleteFaqItemCommandValidator : AbstractValidator<DeleteFaqItemCommand>
    {
        public DeleteFaqItemCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
