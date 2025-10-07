namespace LashStudio.Application.Common.EmailSender.DTO
{
    public sealed record ForgotPasswordDto(string EmailOrUsername, string? RedirectBaseUrl);
    public sealed record ResetPasswordDto(string UserId, string Token, string NewPassword);
}
