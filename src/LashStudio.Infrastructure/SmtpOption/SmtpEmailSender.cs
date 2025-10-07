using LashStudio.Application.Common.Abstractions;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LashStudio.Infrastructure.SmtpOption
{
    public sealed class SmtpEmailSender : IEmailSender
    {
        private readonly SmtpOptions _opt;
        private readonly ILogger<SmtpEmailSender> _log;
        public SmtpEmailSender(IOptions<SmtpOptions> opt, ILogger<SmtpEmailSender> log)
            => (_opt, _log) = (opt.Value, log);

        public async Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            var msg = new MimeMessage();
            msg.From.Add(new MailboxAddress(_opt.FromName, _opt.FromEmail));
            msg.To.Add(MailboxAddress.Parse(to));
            msg.Subject = subject;
            msg.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

            try
            {
                using var client = new SmtpClient(); // для отладки можно: new SmtpClient(new ProtocolLogger("smtp.log"))
                var socket = _opt.UseStartTls ? SecureSocketOptions.StartTls
                                              : SecureSocketOptions.Auto; // либо SslOnConnect для 465

                await client.ConnectAsync(_opt.Host, _opt.Port, socket, ct); // ← ВАЖНО
                await client.AuthenticateAsync(_opt.User, _opt.Password, ct);
                await client.SendAsync(msg, ct);
                await client.DisconnectAsync(true, ct);

                _log.LogInformation("Email sent to {To} ({Subject})", to, subject);
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "SMTP send failed");
                throw; // пусть ваш middleware вернёт 500 с traceId
            }
        }
    }
}