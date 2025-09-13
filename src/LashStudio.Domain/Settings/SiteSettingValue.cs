namespace LashStudio.Domain.Settings
{
    public class SiteSettingValue
    {
        public int Id { get; set; }
        public int SiteSettingId { get; set; }
        public string? Culture { get; set; }            // null/"" = инвариант (для "Currency")
        public string Value { get; set; } = "";
        public SiteSetting? Setting { get; set; }
    }
}
