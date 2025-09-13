namespace LashStudio.Api.Contracts
{
    public record FaqLocaleDto(string Culture, string Question, string Answer);
    public record CreateFaqDto(bool IsActive, int SortOrder, List<FaqLocaleDto> Locales);
    public class UpdateFaqDto
    {
        public bool? IsActive { get; set; }
        public int? SortOrder { get; set; }
        public List<FaqLocaleDto>? Locales { get; set; }
    }
}
