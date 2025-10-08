using System.Globalization;
using LashStudio.Application.Common.Abstractions;
using LashStudio.Application.Common.Localization.Resouresec; // ResetPasswordEmailResources
using LashStudio.Domain.Auth;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace LashStudio.Application.Handlers.Admin.Commands.Email.Commands.ForgotPassword
{
    public sealed class ForgotPasswordHandler : IRequestHandler<ForgotPasswordCommand>
    {
        private readonly UserManager<ApplicationUser> _um;
        private readonly IEmailSender _email;
        private readonly ICurrentStateService _css;

        public ForgotPasswordHandler(
            UserManager<ApplicationUser> um,
            IEmailSender email,
            ICurrentStateService css)
            => (_um, _email, _css) = (um, email, css);

        public async Task Handle(ForgotPasswordCommand r, CancellationToken ct)
        {
            var key = r.Dto.EmailOrUsername?.Trim();
            if (string.IsNullOrWhiteSpace(key)) return;

            var user = await _um.FindByEmailAsync(key) ?? await _um.FindByNameAsync(key);
            if (user is null) return; // не раскрываем существование

            var token = await _um.GeneratePasswordResetTokenAsync(user);

            var baseUrl = string.IsNullOrWhiteSpace(r.Dto.RedirectBaseUrl)
                ? "https://lashschool.example/reset-password"
                : r.Dto.RedirectBaseUrl!.TrimEnd('/');

            var link = $"{baseUrl}?uid={Uri.EscapeDataString(user.Id.ToString())}&toke{Uri.EscapeDataString(token)}";

            // Культура только из CurrentState; fallback — en
            var cultureName = _css?.CurrentCulture ?? CultureInfo.CurrentUICulture?.Name ?? "en";
            CultureInfo culture;
            try { culture = new CultureInfo(cultureName); } catch { culture = new CultureInfo("en"); }

            // Тело письма из ресурсов + подстановка {Link}; fallback на базовый ресурс
            var htmlTpl = ResetPasswordEmailResources.ResourceManager.GetString("ResetPassword", culture)
                        ?? ResetPasswordEmailResources.ResetPassword;

            var html = htmlTpl.Replace("{Link}", link);

            // Фиксированная тема (без ресурсов)
            const string subject = "Reset your password";

            await _email.SendAsync(user.Email!, subject, html, ct);
        }
    }
}
