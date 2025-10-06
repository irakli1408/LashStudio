using FluentValidation;
using LashStudio.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Create
{
    public sealed class CreateCourseCommandValidator : AbstractValidator<CreateCourseCommand>
    {
        // Разрешаем slug в стиле "kebab-case": строчные латинские буквы/цифры и дефисы.
        // Не начинаем/заканчиваем дефисом, без двойных дефисов. Длина 3..80.
        private static readonly Regex SlugRegex =
            new(@"^(?!-)(?!.*--)[a-z0-9-]{3,80}(?<!-)$", RegexOptions.Compiled);

        // Формат культуры вида "ll" или "ll-CC" (пример: "en", "uk-UA").
        private static readonly Regex CultureRegex =
            new(@"^[a-z]{2}(?:-[A-Z]{2})?$", RegexOptions.Compiled);

        public CreateCourseCommandValidator(IAppDbContext db /*, ICurrentStateService? current = null */)
        {
            // Команда должна быть передана
            RuleFor(x => x).NotNull()
                .WithMessage("Command payload is required.");

            // -------------------------------
            // Slug
            // -------------------------------
            RuleFor(x => x.Slug)
                // Обязателен, так как используется как ключ в публичных URL
                .NotEmpty().WithMessage("Slug is required.")
                // Строгий формат: kebab-case, без пробелов/кириллицы/символов
                .Must(s => SlugRegex.IsMatch(s))
                    .WithMessage("Slug must be kebab-case: [a-z0-9-], 3..80 chars, no leading/trailing or double dashes.")
                // Уникальность в рамках таблицы курсов
                .MustAsync(async (slug, ct) =>
                    !await db.Courses.AnyAsync(c => c.Slug == slug, ct))
                    .WithMessage("Slug already exists.");

            // -------------------------------
            // Уровень (Enum)
            // -------------------------------
            RuleFor(x => x.Level)
                // Enum-значение должно попадать в определённые доменом границы
                .IsInEnum().WithMessage("Unknown course level.");

            // -------------------------------
            // Цена
            // -------------------------------
            RuleFor(x => x.Price)
                // Цена может быть отсутствующей (null), либо неотрицательной
                .Must(p => p is null || p >= 0m)
                    .WithMessage("Price must be null or >= 0.")
                // Доп. ограничение от мусора (необязательно): "разумный потолок"
                .Must(p => p is null || p <= 100_000m)
                    .WithMessage("Price is too large.");

            // -------------------------------
            // Продолжительность (часы)
            // -------------------------------
            RuleFor(x => x.DurationHours)
                // Может быть null, либо положительное число
                .Must(h => h is null || h > 0)
                    .WithMessage("DurationHours must be null or > 0.")
                // Доп. "разумный потолок", чтобы не ловить 999999 и т.п.
                .Must(h => h is null || h <= 1000)
                    .WithMessage("DurationHours is too large.");

            // -------------------------------
            // Локали
            // -------------------------------
            RuleFor(x => x.Locales)
                // Должна быть хотя бы одна локаль
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.")
                // Уникальность культур в списке (без дублей)
                .Must(l =>
                    l.Select(i => i.Culture.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase).Count() == l.Count)
                .WithMessage("Locales must have unique cultures (case-insensitive).");

            // Проверяем каждый элемент локали
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                // Культура обязательна, формат — 'll' или 'll-CC'
                // (при необходимости можно сверять с поддерживаемыми культурами через ICurrentStateService)
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.")
                    .Must(c => CultureRegex.IsMatch(c))
                        .WithMessage("Culture must match 'll' or 'll-CC' (e.g., 'en' or 'uk-UA').");
                // .Must(c => current == null || current.SupportedCultures.Contains(c))
                //     .WithMessage(v => $"Unsupported culture: {v.Culture}");

                // Заголовок обязателен и ограничен по длине — главный текст для листинга/деталей
                loc.RuleFor(v => v.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MinimumLength(3).WithMessage("Title is too short (min 3).")
                    .MaximumLength(150).WithMessage("Title is too long (max 150).");

                // Короткое описание — опционально, но ограничиваем длину для UI/SEO
                loc.RuleFor(v => v.ShortDescription)
                    .MaximumLength(500)
                        .When(v => v.ShortDescription is not null)
                        .WithMessage("ShortDescription is too long (max 500).");

                // Полное описание — опционально; добавляем "разумный" потолок
                loc.RuleFor(v => v.FullDescription)
                    .MaximumLength(4000)
                        .When(v => v.FullDescription is not null)
                        .WithMessage("FullDescription is too long (max 4000).");
            });

            // -------------------------------
            // CoverMediaId
            // -------------------------------
            When(x => x.CoverMediaId is not null, () =>
            {
                RuleFor(x => x.CoverMediaId!.Value)
                    // Первичный ключ медиа должен быть > 0
                    .GreaterThan(0).WithMessage("CoverMediaId must be greater than 0.")
                    // Проверяем, что такой Asset существует в БД
                    // (Подстройте под своё хранилище медиа: MediaAssets/MediaFiles/…)
                    .MustAsync(async (assetId, ct) =>
                        await db.MediaAssets.AnyAsync(m => m.Id == assetId, ct))
                    .WithMessage("Cover media asset not found.");
            });
        }
    }
}

