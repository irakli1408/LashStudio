using System.Text.RegularExpressions;

namespace LashStudio.Application.Validation
{
    public static class ValidationPatterns
    {
        // "ll" или "ll-CC" (напр., "en" или "uk-UA")
        public static readonly Regex Culture = new(
            @"^[a-z]{2}(?:-[A-Z]{2})?$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );

        // при желании добавь и другие часто используемые:
        public static readonly Regex Slug = new(
            @"^(?!-)(?!.*--)[a-z0-9-]{3,120}(?<!-)$",
            RegexOptions.Compiled | RegexOptions.CultureInvariant
        );
    }
}
