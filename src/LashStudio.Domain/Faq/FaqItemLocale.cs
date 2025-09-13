namespace LashStudio.Domain.Faq;

public class FaqItemLocale
{
    public long Id { get; set; }
    public long FaqItemId { get; set; }
    public string Culture { get; set; } = "";      // "ru", "en", "uk", "ka"
    public string Question { get; set; } = "";
    public string Answer { get; set; } = "";

    public FaqItem? FaqItem { get; set; }
}
