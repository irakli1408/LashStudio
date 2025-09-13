namespace LashStudio.Api.Contracts.Settings
{
    public record SettingValueDto(string? Culture, string Value);

    public class UpsertSiteSettingDto
    {
        public string Key { get; set; } = "";
        public List<SettingValueDto> Values { get; set; } = new();
    }
}
