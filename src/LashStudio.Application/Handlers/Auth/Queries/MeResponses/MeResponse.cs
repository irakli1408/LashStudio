namespace LashStudio.Application.Handlers.Auth.Queries.MeResponses
{
    public sealed record MeResponse(long UserId, string Email, string? DisplayName, string[] Roles);
}
