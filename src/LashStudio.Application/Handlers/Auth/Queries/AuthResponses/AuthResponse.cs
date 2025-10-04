namespace LashStudio.Application.Handlers.Auth.Queries.AuthResponses
{
    public sealed record AuthResponse(string AccessToken, string RefreshToken, long UserId, string Email, string[] Roles);
}
