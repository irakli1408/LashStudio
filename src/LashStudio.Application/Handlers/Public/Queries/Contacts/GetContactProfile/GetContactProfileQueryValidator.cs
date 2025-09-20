using FluentValidation;
using System.Text.RegularExpressions;

namespace LashStudio.Application.Handlers.Public.Queries.Contacts.GetContactProfile
{
    public sealed class GetContactProfileQueryValidator : AbstractValidator<GetContactProfileQuery>
    {
        // Разрешаем: "ru", "ru-RU", "en", "en-US", "ka", "ka-GE"
        private static readonly Regex CultureRx = new(@"^[a-z]{2}(-[A-Z]{2})?$", RegexOptions.Compiled);

        public GetContactProfileQueryValidator()
        {
            RuleFor(x => x.Culture)
                .MaximumLength(10)
                .Must(v => string.IsNullOrWhiteSpace(v) || CultureRx.IsMatch(v!))
                .WithMessage("Invalid culture format. Use 'xx' or 'xx-XX'.");
        }
    }
}
