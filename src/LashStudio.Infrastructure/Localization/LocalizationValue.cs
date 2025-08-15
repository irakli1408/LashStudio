namespace LashStudio.Infrastructure.Localization;

public class LocalizationValue
{
    public int Id { get; set; }
    public int ResourceId { get; set; }
    public string Culture { get; set; } = "en";    // "en","uk","ka","ru"
    public string Value { get; set; } = "";

    public LocalizationResource Resource { get; set; } = null!;
}
