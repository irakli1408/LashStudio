using LashStudio.Application.Common.Abstractions;
using LashStudio.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ForgotPassword
{
    public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly IEmailSender _email;
        public ForgotPasswordHandler(UserManager<ApplicationUser> um, IEmailSender email)
            => (_um, _email) = (um, email);

        public async Task Handle(ForgotPasswordCommand r, CancellationToken ct)
        {
            var key = r.Dto.EmailOrUsername?.Trim();
            if (string.IsNullOrEmpty(key)) return;

            var user = await _um.FindByEmailAsync(key) ?? await _um.FindByNameAsync(key);
            if (user is null) return; // не раскрываем существование

            var token = await _um.GeneratePasswordResetTokenAsync(user);

            var baseUrl = string.IsNullOrWhiteSpace(r.Dto.RedirectBaseUrl)
                ? "https://lashschool.example/reset-password"
                : r.Dto.RedirectBaseUrl!.TrimEnd('/');

            var link = $"{baseUrl}?uid={user.Id}&token={Uri.EscapeDataString(token)}";

            var html = $@"
                <h3>Сброс пароля</h3>
                <p>Нажмите кнопку, чтобы установить новый пароль:</p>
                <p><a href=""{link}"" style=""display:inline-block;padding:10px 16px;border-radius:8px;text-decoration:none"">
                Сбросить пароль</a></p>
                <p>Если вы не запрашивали сброс — игнорируйте это письмо.</p>";

            await _email.SendAsync(user.Email!, "Reset your password", html, ct);
        }
    }
}
