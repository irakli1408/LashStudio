using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Create
{
    public sealed class CreatePostCommandValidator : AbstractValidator<CreatePostCommand>
    {
        public CreatePostCommandValidator()
        {
            // Должна быть хотя бы одна локаль
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // Уникальные культуры в наборе локалей
            RuleFor(x => x.Locales)
                .Must(l => l.Select(i => i.Culture?.Trim())
                            .Distinct()
                            .Count() == l.Count)
                .WithMessage("Locales must have unique cultures.");

            // Проверки по каждой локали
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                // Культура обязательна
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.");

                // Заголовок обязателен и не слишком длинный
                loc.RuleFor(v => v.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(150).WithMessage("Title is too long (max 150).");

                // Slug обязателен и ограничен по длине (минимально, без сложных regex)
                loc.RuleFor(v => v.Slug)
                    .NotEmpty().WithMessage("Slug is required.")
                    .MaximumLength(120).WithMessage("Slug is too long (max 120).");

                // Контент опционален, но ограничим верх (на случай больших вставок)
                loc.RuleFor(v => v.Content)
                    .MaximumLength(20000)
                    .When(v => v.Content is not null)
                    .WithMessage("Content is too long (max 20000).");
            });

            // Обложка: если задана — положительный Id
            When(x => x.CoverMediaId is not null, () =>
            {
                RuleFor(x => x.CoverMediaId!.Value)
                    .GreaterThan(0).WithMessage("CoverMediaId must be greater than 0.");
            });
        }
    }
}
