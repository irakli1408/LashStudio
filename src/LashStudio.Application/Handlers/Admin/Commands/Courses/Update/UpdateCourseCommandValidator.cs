using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Courses.Update
{
    public sealed class UpdateCourseCommandValidator : AbstractValidator<UpdateCourseCommand>
    {
        public UpdateCourseCommandValidator()
        {
            // Команда и DTO-поля должны быть заданы
            RuleFor(x => x).NotNull()
                .WithMessage("Command payload is required.");

            // Идентификатор курса (PK) > 0
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            // Slug обязателен (доп.ограничения формата/уникальности — вне минималки)
            RuleFor(x => x.Slug)
                .NotEmpty()
                .WithMessage("Slug is required.")
                .MaximumLength(80)
                .WithMessage("Slug is too long (max 80).");

            // Валидное enum-значение уровня
            RuleFor(x => x.Level)
                .IsInEnum()
                .WithMessage("Unknown course level.");

            // Цена: null или неотрицательная
            RuleFor(x => x.Price)
                .Must(p => p is null || p >= 0m)
                .WithMessage("Price must be null or >= 0.");

            // Продолжительность: null или > 0
            RuleFor(x => x.DurationHours)
                .Must(h => h is null || h > 0)
                .WithMessage("DurationHours must be null or > 0.");

            // Локали: хотя бы одна
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // Минимум по каждой локали: культура и заголовок обязательны
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                loc.RuleFor(v => v.Culture)
                    .NotEmpty()
                    .WithMessage("Culture is required.");

                loc.RuleFor(v => v.Title)
                    .NotEmpty()
                    .WithMessage("Title is required.")
                    .MaximumLength(150)
                    .WithMessage("Title is too long (max 150).");

                // Описания опциональны — просто ограничим длину, если заданы
                loc.RuleFor(v => v.ShortDescription)
                    .MaximumLength(500)
                    .When(v => v.ShortDescription is not null)
                    .WithMessage("ShortDescription is too long (max 500).");

                loc.RuleFor(v => v.FullDescription)
                    .MaximumLength(4000)
                    .When(v => v.FullDescription is not null)
                    .WithMessage("FullDescription is too long (max 4000).");
            });

            // Обложка: если указана — PK > 0 (без проверки существования)
            When(x => x.CoverMediaId is not null, () =>
            {
                RuleFor(x => x.CoverMediaId!.Value)
                    .GreaterThan(0)
                    .WithMessage("CoverMediaId must be greater than 0.");
            });
        }
    }
}

