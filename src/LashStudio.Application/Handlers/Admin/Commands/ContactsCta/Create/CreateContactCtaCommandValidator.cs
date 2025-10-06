using FluentValidation;
using LashStudio.Application.Contracts.Contacts;
using System.Text.RegularExpressions;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Create
{
    public sealed class CreateContactCtaCommandValidator : AbstractValidator<CreateContactCtaCommand>
    {
        public CreateContactCtaCommandValidator(/* ICurrentStateService current */)
        {
            // Правим сам объект команды
            RuleFor(x => x).NotNull()
                // Команда вообще должна прийти
                .WithMessage("Command payload is required.");

            // Обязательно должен быть DTO
            RuleFor(x => x.Dto).NotNull()
                .WithMessage("ContactCtaCreateDto is required.");

            // Ниже — правила для полей из Dto
            When(x => x.Dto is not null, () =>
            {
                // Вид кнопки (Enum) — проверяем, что прислано допустимое значение enum
                RuleFor(x => x.Dto!.Kind)
                    .IsInEnum()
                    .WithMessage("Unknown CTA kind.");

                // Порядок сортировки — неотрицательный.
                // Комментарий: это позиция в списке, отрицательные значения неосмысленны.
                RuleFor(x => x.Dto!.Order)
                    .GreaterThanOrEqualTo(0)
                    .WithMessage("Order must be >= 0.");

                // UrlOverride — опциональный, но если задан — должен быть корректным абсолютным http/https URL и не слишком длинным
                RuleFor(x => x.Dto!.UrlOverride)
                    .Cascade(CascadeMode.Stop)
                    .Must(u => string.IsNullOrWhiteSpace(u) || IsHttpUrl(u!))
                        .WithMessage("UrlOverride must be an absolute http/https URL.")
                    .MaximumLength(1024)
                        .When(x => !string.IsNullOrWhiteSpace(x.Dto!.UrlOverride))
                        .WithMessage("UrlOverride is too long.");

                // Локали — обязателен хотя бы один элемент
                RuleFor(x => x.Dto!.Locales)
                    .NotNull().WithMessage("Locales collection is required.")
                    .Must(l => l!.Count > 0).WithMessage("At least one locale is required.")
                    // Уникальность культур в рамках одного CTA
                    .Must(l => l!.Select(i => i.Culture.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count() == l!.Count)
                        .WithMessage("Locales must have unique cultures (case-insensitive).");

                // Правила для каждой локали (культура/лейбл)
                RuleForEach(x => x.Dto!.Locales)
                    .SetValidator(new ContactCtaLocaleUpsertDtoValidator(/* current */));
            });
        }

        // Разрешаем только абсолютные http/https ссылки
        private static bool IsHttpUrl(string s)
        {
            if (!Uri.TryCreate(s, UriKind.Absolute, out var uri)) return false;
            return uri.Scheme is "http" or "https";
        }
    }

    /// <summary>
    /// Валидация локали CTA.
    /// </summary>
    public sealed class ContactCtaLocaleUpsertDtoValidator : AbstractValidator<ContactCtaLocaleUpsertDto>
    {
        // Культура: ll или ll-CC (пример: "en", "uk-UA")
        private static readonly Regex CulturePattern = new(@"^[a-z]{2}(?:-[A-Z]{2})?$", RegexOptions.Compiled);

        public ContactCtaLocaleUpsertDtoValidator(/* ICurrentStateService current */)
        {
            // Культура обязательна, формат проверяем регуляркой.
            // (Если у вас есть ICurrentStateService.SupportedCultures — можно дополнительно сверять принадлежность.)
            RuleFor(x => x.Culture)
                .NotEmpty().WithMessage("Culture is required.")
                .Must(c => CulturePattern.IsMatch(c))
                    .WithMessage("Culture must match 'll' or 'll-CC' (e.g., 'en' or 'uk-UA').");
            // .Must(c => current.SupportedCultures.Contains(c))
            //     .WithMessage(x => $"Unsupported culture: {x.Culture}");

            // Label — это пользовательский текст кнопки, обязателен и ограничен по длине
            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Label is required.")
                .MaximumLength(100).WithMessage("Label is too long (max 100).");
        }
    }
}

