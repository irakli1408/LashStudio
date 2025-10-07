using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public sealed class UpdateFaqItemCommandValidator : AbstractValidator<UpdateFaqItemCommand>
    {
        public UpdateFaqItemCommandValidator()
        {
            RuleFor(x => x.Body.Id).GreaterThan(0);
            RuleFor(x => x.Body.Locales).NotEmpty();

            RuleForEach(x => x.Body.Locales).ChildRules(v =>
            {
                v.RuleFor(l => l.Culture).NotEmpty().MaximumLength(10);
                v.RuleFor(l => l.Question).NotEmpty().MaximumLength(500);
                v.RuleFor(l => l.Answer).NotEmpty().MaximumLength(4000);
            });
        }
    }
}
