using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Posts.Update
{
    public sealed class UpdatePostCommandValidator : AbstractValidator<UpdatePostCommand>
    {
        public UpdatePostCommandValidator()
        {
            // Идентификатор поста должен быть > 0
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");

            // Должна быть хотя бы одна локаль
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // Уникальные культуры среди локалей
            RuleFor(x => x.Locales)
                .Must(l => l.Select(i => i.Culture?.Trim())
                            .Distinct()
                            .Count() == l.Count)
                .WithMessage("Locales must have unique cultures.");

            // Проверки для каждой локали
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.");

                loc.RuleFor(v => v.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(150).WithMessage("Title is too long (max 150).");

                loc.RuleFor(v => v.Slug)
                    .NotEmpty().WithMessage("Slug is required.")
                    .MaximumLength(120).WithMessage("Slug is too long (max 120).");

                loc.RuleFor(v => v.Content)
                    .MaximumLength(20000)
                    .When(v => v.Content is not null)
                    .WithMessage("Content is too long (max 20000).");
            });

            // Обложка: если указана — положительный Id
            When(x => x.CoverMediaId is not null, () =>
            {
                RuleFor(x => x.CoverMediaId!.Value)
                    .GreaterThan(0).WithMessage("CoverMediaId must be greater than 0.");
            });
        }
    }
}

