using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Faq.Create
{
    public sealed class CreateFaqItemCommandValidator : AbstractValidator<CreateFaqItemCommand>
    {
        public CreateFaqItemCommandValidator()
        {
            // Команда должна быть передана
            RuleFor(x => x).NotNull()
                .WithMessage("Command payload is required.");

            // SortOrder >= 0 (обычно сортировка неотрицательная)
            RuleFor(x => x.SortOrder)
                .GreaterThanOrEqualTo(0)
                .WithMessage("SortOrder must be >= 0.");

            // Должна быть хотя бы одна локаль
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // (Лёгкая проверка) Культуры без дублей — чтобы не было двух записей на одну культуру
            RuleFor(x => x.Locales)
                .Must(l => l.Select(i => i.Culture?.Trim()).Distinct().Count() == l.Count)
                .WithMessage("Locales must have unique cultures.");

            // Минимум по каждой локали: обязательные поля
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.");

                loc.RuleFor(v => v.Question)
                    .NotEmpty().WithMessage("Question is required.");

                loc.RuleFor(v => v.Answer)
                    .NotEmpty().WithMessage("Answer is required.");
            });
        }
    }
}
