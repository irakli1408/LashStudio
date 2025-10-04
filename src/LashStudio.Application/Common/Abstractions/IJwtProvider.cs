namespace LashStudio.Application.Common.Abstractions
{
    public interface IJwtProvider
    {
        string CreateAccessToken(long userId, string email, IEnumerable<string> roles);
    }
}
