namespace LashStudio.Infrastructure.Localization;

public class LocalizationResource
{
    public int Id { get; set; }
    public string Key { get; set; } = "";   
    public string? Description { get; set; }     
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<LocalizationValue> Values { get; set; } = new List<LocalizationValue>();
}
