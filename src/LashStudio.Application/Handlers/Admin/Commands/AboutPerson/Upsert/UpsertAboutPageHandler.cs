using FluentValidation;
using LashStudio.Application.Contracts.AboutPerson;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
{
    /// <summary>
    /// Валидатор именно для команды UpsertAboutPageCommand.
    /// Вызывается автоматически через ValidationBehavior до хендлера.
    /// </summary>
    internal sealed class UpsertAboutPageCommandValidator : AbstractValidator<UpsertAboutPageCommand>
    {
        public UpsertAboutPageCommandValidator()
        {
            // 1) Модель обязательна
            // Причина: в хендлере q.Model используется без null-check'ов.
            RuleFor(x => x.Model).NotNull();

            // 2) SEO-блок: поля опциональны, но если переданы — ограничиваем длину
            // Причина: не переполнять БД/метатеги и держать данные в узких рамках.
            When(x => !string.IsNullOrWhiteSpace(x.Model.SeoTitle), () =>
            {
                RuleFor(x => x.Model.SeoTitle!)
                    .MaximumLength(200);
            });

            When(x => !string.IsNullOrWhiteSpace(x.Model.SeoDescription), () =>
            {
                RuleFor(x => x.Model.SeoDescription!)
                    .MaximumLength(500); // мета-описания обычно короткие
            });

            // CSV-ключевые слова: допускаем null/пусто; если есть — базовая гигиена.
            RuleFor(x => x.Model.SeoKeywordsCsv)
                .Must(csv => IsValidKeywordsCsv(csv, maxKeywords: 20, maxWordLen: 50))
                .WithMessage("SEO keywords: up to 20 unique items, each 1–50 chars.");

            // 3) Локали обязательны и не пусты
            // Причина: страница «Обо мне» без локалей не имеет смысла;
            // хендлер синхронизирует локали и удаляет отсутствующие.
            RuleFor(x => x.Model.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l!.Count > 0).WithMessage("At least one locale is required.");

            // 3.1) Каждую локаль валидируем отдельным валидатором
            RuleForEach(x => x.Model.Locales!)
                .SetValidator(new AboutPageLocaleDtoValidator());

            // 3.2) Уникальность культур (case-insensitive)
            // Причина: хендлер строит словарь по Culture — дубликаты сломают синхронизацию.
            RuleFor(x => x.Model.Locales!)
                .Must(list =>
                {
                    var cultures = list
                        .Select(l => l.Culture?.Trim())
                        .Where(s => !string.IsNullOrWhiteSpace(s))!
                        .ToList();

                    return cultures.Distinct(StringComparer.OrdinalIgnoreCase).Count() == cultures.Count;
                })
                .WithMessage("Locales must have distinct Culture codes.");
        }

        private static bool IsValidKeywordsCsv(string? csv, int maxKeywords, int maxWordLen)
        {
            if (string.IsNullOrWhiteSpace(csv)) return true;
            var parts = csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (parts.Length > maxKeywords) return false;
            // уникальность без учёта регистра
            if (parts.Distinct(StringComparer.OrdinalIgnoreCase).Count() != parts.Length) return false;
            // длина каждого слова 1..maxWordLen
            return parts.All(k => k.Length >= 1 && k.Length <= maxWordLen);
        }
    }

    /// <summary>
    /// Валидация одной локали AboutPage.
    /// </summary>
    internal sealed class AboutPageLocaleDtoValidator : AbstractValidator<AboutLocaleDto>
    {
        public AboutPageLocaleDtoValidator()
        {
            // Culture обязателен и должен быть вида "xx" или "xx-XX" (например, "uk", "uk-UA", "ka-GE").
            // Причина: хендлер ищет локали точным сравнением строк.
            RuleFor(x => x.Culture)
                .NotEmpty().WithMessage("Culture is required.")
                .Matches("^[a-z]{2}(-[A-Z]{2})?$")
                .WithMessage("Culture must look like 'en' or 'en-US'.");

            // Title обязателен (H1/основной заголовок).
            // Причина: пустая шапка страницы — плохой UX/SEO.
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            // SubTitle — опционален, но ограничим длину, чтобы не «распухало».
            RuleFor(x => x.SubTitle)
                .MaximumLength(300).When(x => !string.IsNullOrWhiteSpace(x.SubTitle));

            // BodyHtml обязателен — основной контент страницы.
            // Если допускаешь пустой контент — смягчи до NotNull().
            RuleFor(x => x.BodyHtml)
                .NotEmpty().WithMessage("BodyHtml is required.")
                .MaximumLength(20000); // подгони под размер колонки, если другой
        }
    }
}