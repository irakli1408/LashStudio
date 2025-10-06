using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Update
{
    public sealed class UpdateFaqItemCommandValidator : AbstractValidator<UpdateFaqItemCommand>
    {
        public UpdateFaqItemCommandValidator()
        {
            // Обязателен валидный PK
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            // SortOrder: если передан — неотрицательный
            When(x => x.SortOrder.HasValue, () =>
            {
                RuleFor(x => x.SortOrder!.Value)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("SortOrder must be >= 0.");
            });

            // Locales: если переданы — не пустые и без дубликатов культур
            When(x => x.Locales is not null, () =>
            {
                RuleFor(x => x.Locales!)
                    .Must(l => l.Count > 0)
                    .WithMessage("Locales must not be empty when provided.")
                    .Must(l => l.Select(i => i.Culture?.Trim())
                                .Distinct()
                                .Count() == l.Count)
                    .WithMessage("Locales must have unique cultures.");

                // Для каждой локали — минимально обязательные поля
                RuleForEach(x => x.Locales!).ChildRules(loc =>
                {
                    loc.RuleFor(v => v.Culture)
                        .NotEmpty()
                        .WithMessage("Culture is required.");

                    loc.RuleFor(v => v.Question)
                        .NotEmpty()
                        .WithMessage("Question is required.");

                    loc.RuleFor(v => v.Answer)
                        .NotEmpty()
                        .WithMessage("Answer is required.");
                });
            });
        }
    }
}
