namespace LashStudio.Application.Contracts
{
    public readonly record struct UserInfo(long Id, string Email, string? DisplayName);
}
