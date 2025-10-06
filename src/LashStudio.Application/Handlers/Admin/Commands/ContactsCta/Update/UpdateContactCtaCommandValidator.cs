using FluentValidation;

namespace LashStudio.Application.Handlers.Admin.Commands.ContactsCta.Update
{
    public sealed class UpdateContactCtaCommandValidator : AbstractValidator<UpdateContactCtaCommand>
    {
        public UpdateContactCtaCommandValidator()
        {
            // Команда и DTO должны быть переданы
            RuleFor(x => x).NotNull().WithMessage("Command payload is required.");
            RuleFor(x => x.Dto).NotNull().WithMessage("ContactCtaUpdateDto is required.");

            When(x => x.Dto is not null, () =>
            {
                // Id > 0: PK не может быть 0/отрицательным
                RuleFor(x => x.Dto!.Id)
                    .GreaterThan(0).WithMessage("Id must be greater than 0.");

                // Kind — enum: защищаемся от невалидных значений
                RuleFor(x => x.Dto!.Kind)
                    .IsInEnum().WithMessage("Unknown CTA kind.");

                // UrlOverride — опционально; если задан, должен быть http/https и разумной длины
                RuleFor(x => x.Dto!.UrlOverride)
                    .Cascade(CascadeMode.Stop)
                    .Must(u => string.IsNullOrWhiteSpace(u) || IsHttpUrl(u!))
                        .WithMessage("UrlOverride must be an absolute http/https URL.")
                    .MaximumLength(1024)
                        .When(x => !string.IsNullOrWhiteSpace(x.Dto!.UrlOverride))
                        .WithMessage("UrlOverride is too long (max 1024).");
            });
        }

        private static bool IsHttpUrl(string s)
        {
            if (!Uri.TryCreate(s, UriKind.Absolute, out var uri)) return false;
            return uri.Scheme is "http" or "https";
        }
    }
}
