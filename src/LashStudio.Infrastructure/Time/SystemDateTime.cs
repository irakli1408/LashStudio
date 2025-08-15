using LashStudio.Application.Common.Abstractions;

namespace LashStudio.Infrastructure.Time;

public sealed class SystemDateTime : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
}
