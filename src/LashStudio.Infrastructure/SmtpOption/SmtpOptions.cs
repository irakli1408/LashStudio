﻿namespace LashStudio.Infrastructure.SmtpOption
{
    public sealed class SmtpOptions
    {
        public string Host { get; init; } = default!;
        public int Port { get; init; } = 587;
        public bool UseStartTls { get; init; } = true;
        public string User { get; init; } = default!;
        public string Password { get; init; } = default!;
        public string FromEmail { get; init; } = default!;
        public string FromName { get; init; } = "LashSchool";
    }
}
