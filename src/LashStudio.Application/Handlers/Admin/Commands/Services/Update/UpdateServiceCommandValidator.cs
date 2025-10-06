using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.Services.Update
{
    public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
    {
        public UpdateServiceCommandValidator()
        {
            // Валиден ли PK (Guid не пустой)
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id must be a non-empty GUID.");

            // Slug обязателен и не слишком длинный (минимально)
            RuleFor(x => x.Slug)
                .NotEmpty().WithMessage("Slug is required.")
                .MaximumLength(80).WithMessage("Slug is too long (max 80).");

            // Категория — валидный enum
            RuleFor(x => x.Category)
                .IsInEnum().WithMessage("Unknown service category.");

            // Вариант (nullable enum): если задан — валиден
            When(x => x.Variant.HasValue, () =>
            {
                RuleFor(x => x.Variant!.Value)
                    .IsInEnum().WithMessage("Unknown lash extension variant.");
            });

            // Цена > 0
            RuleFor(x => x.Price)
                .GreaterThan(0m).WithMessage("Price must be greater than 0.");

            // Длительность: null или > 0
            RuleFor(x => x.DurationMinutes)
                .Must(d => d is null || d > 0)
                .WithMessage("DurationMinutes must be null or > 0.");

            // Должна быть хотя бы одна локаль
            RuleFor(x => x.Locales)
                .NotNull().WithMessage("Locales are required.")
                .Must(l => l.Count > 0).WithMessage("At least one locale is required.");

            // Культуры без дублей
            RuleFor(x => x.Locales)
                .Must(l => l.Select(i => i.Culture?.Trim())
                            .Distinct()
                            .Count() == l.Count)
                .WithMessage("Locales must have unique cultures.");

            // Минимальные проверки по каждой локали
            RuleForEach(x => x.Locales).ChildRules(loc =>
            {
                loc.RuleFor(v => v.Culture)
                    .NotEmpty().WithMessage("Culture is required.");

                loc.RuleFor(v => v.Title)
                    .NotEmpty().WithMessage("Title is required.")
                    .MaximumLength(150).WithMessage("Title is too long (max 150).");

                loc.RuleFor(v => v.ShortDescription)
                    .MaximumLength(500)
                    .When(v => v.ShortDescription is not null)
                    .WithMessage("ShortDescription is too long (max 500).");

                loc.RuleFor(v => v.FullDescription)
                    .MaximumLength(4000)
                    .When(v => v.FullDescription is not null)
                    .WithMessage("FullDescription is too long (max 4000).");
            });
        }
    }
}
