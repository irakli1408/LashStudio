namespace LashStudio.Api.Config;

public class LocalizationOptions
{
    public string DefaultCulture { get; set; } = "en";
    public string[] SupportedCultures { get; set; } = new[] { "en" };
}
