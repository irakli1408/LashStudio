namespace LashStudio.Api.Contracts.Posts
{
    public record PostLocaleDto(string Culture, string Title, string Slug, string Content);
    public record UpdatePostDto(List<PostLocaleDto> Locales, long? CoverMediaId);
    public record LocaleDto(string Culture, string Title, string Slug, string? Content);
    public record CreatePostDto(List<LocaleDto> Locales, long? CoverMediaId);
}
