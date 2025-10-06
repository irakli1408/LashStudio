using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.SetContactCta
{
    public sealed class SetContactCtaEnabledCommandValidator : AbstractValidator<SetContactCtaEnabledCommand>
    {
        public SetContactCtaEnabledCommandValidator()
        {
            // Идентификатор обязателен и > 0.
            // Зачем: 0/минус не соответствуют реальным PK и сигнализируют о неверном запросе клиента.
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("Id must be greater than 0.");
        }
    }
}
