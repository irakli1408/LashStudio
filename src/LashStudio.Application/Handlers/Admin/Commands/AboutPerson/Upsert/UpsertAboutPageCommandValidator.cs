using FluentValidation;
using LashStudio.Application.Contracts.AboutPerson;

namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
{
    namespace LashStudio.Application.Handlers.Admin.Commands.AboutPerson.Upsert
    {
        // Валидируем саму команду, т.к. через нее приходит вся модель
        internal sealed class UpsertAboutPageCommandValidator : AbstractValidator<UpsertAboutPageCommand>
        {
            public UpsertAboutPageCommandValidator()
            {
                // 1) Модель обязана прийти
                // Причина: handler без null-check на q.Model, сразу читает поля.
                RuleFor(x => x.Model).NotNull();

                // 2) SEO-блок: допускаем пустые поля, НО если заполнены — ограничиваем длину
                // Причина: защита БД/поисковых сниппетов и UX (короткие метатеги).
                When(x => !string.IsNullOrWhiteSpace(x.Model.SeoTitle), () =>
                {
                    RuleFor(x => x.Model.SeoTitle!)
                        .MaximumLength(200); // подгони к длине колонки в БД, если другая
                });

                When(x => !string.IsNullOrWhiteSpace(x.Model.SeoDescription), () =>
                {
                    RuleFor(x => x.Model.SeoDescription!)
                        .MaximumLength(500); // meta description обычно до ~160-320 символов
                });

                // CSV-ключевые слова: необязательно, но если есть — ограничим размер и валидируем формат
                RuleFor(x => x.Model.SeoKeywordsCsv)
                    .Must(csv => csv == null
                        || csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .All(k => k.Length is > 0 and <= 50))
                    .WithMessage("Each SEO keyword must be 1–50 chars.")
                    .Must(csv => csv == null
                        || csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                              .Distinct(StringComparer.OrdinalIgnoreCase).Count()
                           == csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length)
                    .WithMessage("SEO keywords must be unique (case-insensitive).")
                    .Must(csv => csv == null
                        || csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).Length <= 20)
                    .WithMessage("No more than 20 SEO keywords.");

                // 3) Locales: это ядро AboutPage — требуем хотя бы одну локаль
                // Причина: handler синхронизирует локали и удаляет отсутствующие — пустой набор сломает контент страницы.
                RuleFor(x => x.Model.Locales)
                    .NotNull().WithMessage("Locales are required.")
                    .Must(l => l!.Count > 0).WithMessage("At least one locale is required.");

                // 3.1) У каждой локали валидируем поля
                RuleForEach(x => x.Model.Locales!)
        .SetValidator(new AboutPageLocaleDtoValidator());

                // 3.2) Уникальность культур среди входных локалей
                // Причина: handler строит словарь по Culture — дубликаты приведут к конфликтам.
                RuleFor(x => x.Model.Locales!)
                    .Must(list =>
                    {
                        var cultures = list.Select(l => l.Culture?.Trim()).Where(s => !string.IsNullOrWhiteSpace(s));
                        return cultures.Distinct(StringComparer.OrdinalIgnoreCase).Count() == cultures.Count();
                    })
                    .WithMessage("Locales must have distinct Culture codes.");
            }
        }

        // Валидатор одной локали AboutPage
        internal sealed class AboutPageLocaleDtoValidator : AbstractValidator<AboutLocaleDto>
        {
            public AboutPageLocaleDtoValidator()
            {
                // Culture обязателен и в формате xx или xx-XX (en, uk, ka-GE и т.п.)
                // Причина: handler использует точное совпадение строк для синхронизации.
                RuleFor(x => x.Culture)
                    .NotEmpty().WithMessage("Culture is required.")
                    .Matches("^[a-z]{2}(-[A-Z]{2})?$")
                    .WithMessage("Culture must look like 'en' or 'en-US'.");

                // Title обязателен (это H1/основной заголовок страницы «Обо мне»)
                RuleFor(x => x.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(200);

                // SubTitle — опционален, но ограничим длину
                RuleFor(x => x.SubTitle)
                    .MaximumLength(300).When(x => !string.IsNullOrWhiteSpace(x.SubTitle));

                // BodyHtml — обязателен (основной контент). Пустая страница «Обо мне» бессмысленна.
                // Если допускаете пустой текст — смягчите правило до .NotNull()
                RuleFor(x => x.BodyHtml)
                    .NotEmpty().WithMessage("BodyHtml is required.")
                    // Дополнительно: ограничить верхнюю границу (например, 20000), если есть лимит колонки/бизнес-ограничение.
                    .MaximumLength(20000);
            }
        }
    }
}
