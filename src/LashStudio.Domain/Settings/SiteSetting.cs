using LashStudio.Domain.Settings;

public class SiteSetting
{
    public int Id { get; set; }
    public string Key { get; set; } = "";           // "Currency", "PriceNote"
    public List<SiteSettingValue> Values { get; set; } = new();
}