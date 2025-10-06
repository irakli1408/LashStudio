using FluentValidation;
using System.Text.RegularExpressions;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Upsert
{
    public sealed class UpsertContactCtaLocalesCommandValidator : AbstractValidator<UpsertContactCtaLocalesCommand>
    {
        // Разрешаем культуры вида "ll" или "ll-CC" (пример: "en", "uk-UA").
        private static readonly Regex CulturePattern = new(@"^[a-z]{2}(?:-[A-Z]{2})?$", RegexOptions.Compiled);

        public UpsertContactCtaLocalesCommandValidator()
        {
            // Идентификатор CTA обязателен и > 0.
            // Зачем: 0/отрицательные значения неприемлемы для PK.
            RuleFor(x => x.CtaId)
                .GreaterThan(0)
                .WithMessage("CtaId must be greater than 0.");

            // Список локалей обязателен и не пуст.
            // Зачем: апсерту есть смысл только при наличии хотя бы одной локали.
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // Культуры внутри списка уникальны (без дубликатов).
            // Зачем: апдейт/инсерт по Culture — дубликаты сделают поведение неоднозначным.
            RuleFor(x => x.Locales)
                .Must(l => l.Select(i => i.Culture.Trim())
                            .Distinct(StringComparer.OrdinalIgnoreCase).Count() == l.Count)
                .WithMessage("Locales must have unique cultures (case-insensitive).");

            // Валидация каждого элемента локали.
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                // Культура обязательна и соответствует формату.
                // Зачем: гарантируем валидный CultureTag.
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.")
                    .Must(c => CulturePattern.IsMatch(c))
                    .WithMessage("Culture must match 'll' or 'll-CC' (e.g., 'en' or 'uk-UA').");

                // Label обязателен, ограничиваем длину для UI/хранения.
                // Зачем: защита от слишком длинных значений и пустых лейблов.
                loc.RuleFor(v => v.Label)
                    .NotEmpty().WithMessage("Label is required.")
                    .MaximumLength(100).WithMessage("Label is too long (max 100).");
            });
        }
    }
}
