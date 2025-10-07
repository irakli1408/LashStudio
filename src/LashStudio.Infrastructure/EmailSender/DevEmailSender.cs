using LashStudio.Application.Common.Abstractions;
using Microsoft.Extensions.Logging;

namespace LashStudio.Infrastructure.EmailSender
{
    public sealed class DevEmailSender : IEmailSender
    {
        private readonly ILogger<DevEmailSender> _log;
        public DevEmailSender(ILogger<DevEmailSender> log) => _log = log;
        public Task SendAsync(string to, string subject, string htmlBody, CancellationToken ct = default)
        {
            _log.LogWarning("DEV MAIL to={To}\nSUBJ={Subj}\nBODY={Body}", to, subject, htmlBody);
            return Task.CompletedTask;
        }
    }
}
