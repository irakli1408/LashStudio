using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Reorder
{
    public sealed class ReorderContactCtasCommandValidator : AbstractValidator<ReorderContactCtasCommand>
    {
        public ReorderContactCtasCommandValidator()
        {
            // Команда должна прийти
            RuleFor(x => x).NotNull()
                .WithMessage("Command payload is required.");

            // Список обязателен
            RuleFor(x => x.OrderedIds)
                .NotNull().WithMessage("OrderedIds are required.")
                .Must(l => l.Count > 0).WithMessage("Provide at least one id.");

            // Все Id > 0
            // Зачем: первичные ключи не бывают 0/отрицательными.
            RuleForEach(x => x.OrderedIds)
                .Must(id => id > 0)
                .WithMessage("All ids must be greater than 0.");

            // Уникальность Id
            // Зачем: дубликаты сломают перестановку (двум элементам присвоится один и тот же порядок).
            RuleFor(x => x.OrderedIds)
                .Must(l => l.Distinct().Count() == l.Count)
                .WithMessage("Ids must be unique.");
        }
    }
}
