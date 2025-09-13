namespace LashStudio.Application.Handlers.Admin.Commands.Posts
{
    public record PostLocaleInput(string Culture, string Title, string Slug, string? Content);
}
