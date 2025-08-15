namespace LashStudio.Application.Common.Abstractions;

public interface IDateTime
{
    DateTime UtcNow { get; }
}