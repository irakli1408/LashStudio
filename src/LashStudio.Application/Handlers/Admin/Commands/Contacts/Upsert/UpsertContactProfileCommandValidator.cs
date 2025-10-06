using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Contacts.Upsert
{
    internal sealed class UpsertContactProfileCommandValidator : AbstractValidator<UpsertContactProfileCommand>
    {
        public UpsertContactProfileCommandValidator()
        {
            // 0) Dto должен прийти (хендлер сразу обращается к r.Dto.*).
            RuleFor(x => x.Dto).NotNull();

            // 1) EMAILS (опциональны, но если есть — корректный формат и разумная длина).
            RuleFor(x => x.Dto.EmailPrimary)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Dto.EmailPrimary))
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Dto.EmailPrimary))
                // почему так: e-mail может быть пустым, но если пользователь его даёт — пусть будет валидным и не слишком длинным.
                ;

            RuleFor(x => x.Dto.EmailSales)
                .Cascade(CascadeMode.Stop)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Dto.EmailSales))
                .EmailAddress().When(x => !string.IsNullOrWhiteSpace(x.Dto.EmailSales));

            // 2) PHONES (массив обязателен по типу, допускаем пустой список).
            // Каждый телефон: только цифры, пробелы, (), +, -, длина 5–20.
            RuleForEach(x => x.Dto.Phones)
                .Matches(@"^[0-9+\-\s()]{5,20}$")
                .WithMessage("Phone number format invalid (allowed: +, digits, spaces, (), -).");
            // почему так: единая гигиена формата, но без жёсткой E.164 — обычно хватает.

            // 3) SOCIALS (опциональны; хендлер сам отрежет '@' у Instagram/Telegram).
            RuleFor(x => x.Dto.Instagram)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Dto.Instagram))
                .Matches(@"^[A-Za-z0-9_.]+$").When(x => !string.IsNullOrWhiteSpace(x.Dto.Instagram))
                .WithMessage("Instagram handle may contain only letters, digits, underscores and dots.");
            // почему так: инсты без пробелов/символов; длина с запасом.

            RuleFor(x => x.Dto.Telegram)
                .MaximumLength(100).When(x => !string.IsNullOrWhiteSpace(x.Dto.Telegram))
                .Matches(@"^[A-Za-z0-9_]+$").When(x => !string.IsNullOrWhiteSpace(x.Dto.Telegram))
                .WithMessage("Telegram handle may contain only letters, digits and underscores.");

            RuleFor(x => x.Dto.WhatsApp)
                .MaximumLength(20).When(x => !string.IsNullOrWhiteSpace(x.Dto.WhatsApp))
                .Matches(@"^[0-9+\-\s()]+$").When(x => !string.IsNullOrWhiteSpace(x.Dto.WhatsApp))
                .WithMessage("WhatsApp number format invalid.");
            // почему так: как телефон, но без обязательной длины 5 — WA может хранить и короткие тестовые.

            // 4) MAP (в DTO lat/lng — decimal?, zoom — int (обязателен)).
            RuleFor(x => x.Dto.MapLat)
                .InclusiveBetween(-90, 90).When(x => x.Dto.MapLat is not null)
                .WithMessage("Map latitude must be between -90 and 90.");

            RuleFor(x => x.Dto.MapLng)
                .InclusiveBetween(-180, 180).When(x => x.Dto.MapLng is not null)
                .WithMessage("Map longitude must be between -180 and 180.");

            RuleFor(x => x.Dto.MapZoom)
                .InclusiveBetween(0, 25)
                .WithMessage("Map zoom should be 0–25.");
            // почему так: тип int обязателен — даём безопасный диапазон тайловых карт.

            // 5) HOURS (опционально; если пришли — валидируем каждый элемент).
            When(x => x.Dto.Hours is not null && x.Dto.Hours.Count > 0, () =>
            {
                RuleForEach(x => x.Dto.Hours!)
                    .SetValidator(new ContactBusinessHourDtoValidator());
            });
            // почему так: у закрытого дня часы не требуем, у открытого — проверяем формат и что Close > Open.

            // 6) LOCALES (обязательны: IReadOnlyList<ContactProfileLocaleDto> Locales).
            RuleFor(x => x.Dto.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l!.Count > 0).WithMessage("At least one locale is required.");

            RuleForEach(x => x.Dto.Locales!)
                .SetValidator(new ContactProfileLocaleDtoValidator());

            // Уникальность культур без учёта регистра, чтобы handler корректно собрал коллекцию.
            RuleFor(x => x.Dto.Locales!)
                .Must(list =>
                {
                    var cultures = list.Select(l => l.Culture?.Trim())
                                       .Where(s => !string.IsNullOrWhiteSpace(s))!
                                       .ToList();
                    return cultures.Distinct(StringComparer.OrdinalIgnoreCase).Count() == cultures.Count;
                })
                .WithMessage("Locales must have distinct Culture codes.");

            // 7) SEO (опциональны, но ограничиваем длину, как и в About/Contacts)
            RuleFor(x => x.Dto.SeoTitle)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.Dto.SeoTitle));

            RuleFor(x => x.Dto.SeoDescription)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.Dto.SeoDescription));
            // почему так: короткие мета-теги и безопасные для БД длины.
        }
    }

    /// <summary>
    /// Валидатор локали профиля контактов.
    /// Все поля (кроме Culture) в DTO опциональны — не завышаем строгость.
    /// </summary>
    internal sealed class ContactProfileLocaleDtoValidator : AbstractValidator<ContactProfileLocaleDto>
    {
        public ContactProfileLocaleDtoValidator()
        {
            // Culture обязателен и в корректном формате (xx или xx-XX)
            RuleFor(x => x.Culture)
                .NotEmpty().WithMessage("Culture is required.")
                .Matches("^[a-z]{2}(-[A-Z]{2})?$")
                .WithMessage("Culture must look like 'en' or 'en-US'.");

            // Остальные поля — опциональны, но ограничим длины, чтобы не «распухали».
            RuleFor(x => x.OrganizationName)
                .MaximumLength(200).When(x => !string.IsNullOrWhiteSpace(x.OrganizationName));

            RuleFor(x => x.AddressLine)
                .MaximumLength(300).When(x => !string.IsNullOrWhiteSpace(x.AddressLine));

            RuleFor(x => x.HowToFindUs)
                .MaximumLength(500).When(x => !string.IsNullOrWhiteSpace(x.HowToFindUs));
            // почему так: соответствуем гибкости DTO — не навязываем обязательность там, где модель позволяет null.
        }
    }

    /// <summary>
    /// Валидатор рабочего времени.
    /// </summary>
    internal sealed class ContactBusinessHourDtoValidator : AbstractValidator<ContactBusinessHourDto>
    {
        public ContactBusinessHourDtoValidator()
        {
            // DayOfWeek enum — тип гарантирует валидность значения, отдельный диапазон не нужен.

            // Если IsClosed == true, Open/Close могут быть null/пустыми.
            When(x => x.IsClosed, () =>
            {
                RuleFor(x => x.Open).Must(o => string.IsNullOrWhiteSpace(o))
                    .WithMessage("Open must be empty when closed.");
                RuleFor(x => x.Close).Must(c => string.IsNullOrWhiteSpace(c))
                    .WithMessage("Close must be empty when closed.");
            });

            // Если IsClosed == false — оба времени обязательны и валидны, а Close позже Open.
            When(x => !x.IsClosed, () =>
            {
                RuleFor(x => x.Open)
                    .NotEmpty().WithMessage("Open time is required when not closed.")
                    .Matches(@"^\d{2}:\d{2}$").WithMessage("Open must be in HH:mm.");

                RuleFor(x => x.Close)
                    .NotEmpty().WithMessage("Close time is required when not closed.")
                    .Matches(@"^\d{2}:\d{2}$").WithMessage("Close must be in HH:mm.");

                RuleFor(x => x)
                    .Must(x =>
                    {
                        // безопасно сравниваем через TimeOnly.TryParse
                        if (!TimeOnly.TryParse(x.Open, out var o)) return false;
                        if (!TimeOnly.TryParse(x.Close, out var c)) return false;
                        return c > o;
                    })
                    .WithMessage("Close must be later than Open.");
            });
            // почему так: отражаем бизнес-смысл — у открытых дней график обязателен и непротиворечив.
        }
    }
}
