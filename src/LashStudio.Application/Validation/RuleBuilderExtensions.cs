using FluentValidation;

namespace LashStudio.Application.Validation
{
    public static class RuleBuilderExtensions
    {
        /// <summary>Опциональная культура: пусто ОК, иначе валидный формат.</summary>
        public static IRuleBuilderOptions<T, string?> OptionalCulture<T>(this IRuleBuilder<T, string?> rule) =>
            rule
                .MaximumLength(10)
                .Must(v => string.IsNullOrWhiteSpace(v) || ValidationPatterns.Culture.IsMatch(v!))
                .WithMessage("Invalid culture format. Use 'xx' or 'xx-XX'.");

        /// <summary>Обязательная культура.</summary>
        public static IRuleBuilderOptions<T, string> RequiredCulture<T>(this IRuleBuilder<T, string> rule) =>
            rule
                .NotEmpty().WithMessage("Culture is required.")
                .MaximumLength(10)
                .Must(v => ValidationPatterns.Culture.IsMatch(v))
                .WithMessage("Invalid culture format. Use 'xx' or 'xx-XX'.");

        /// <summary>Опциональный slug: пусто ОК, иначе формат.</summary>
        public static IRuleBuilderOptions<T, string?> OptionalSlug<T>(this IRuleBuilder<T, string?> rule) =>
            rule
                .MaximumLength(120)
                .Must(v => string.IsNullOrWhiteSpace(v) || ValidationPatterns.Slug.IsMatch(v!))
                .WithMessage("Invalid slug format.");

        /// <summary>Обязательный slug.</summary>
        public static IRuleBuilderOptions<T, string> RequiredSlug<T>(this IRuleBuilder<T, string> rule) =>
            rule
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(120)
                .Must(v => ValidationPatterns.Slug.IsMatch(v))
                .WithMessage("Invalid slug format.");
    }
}
