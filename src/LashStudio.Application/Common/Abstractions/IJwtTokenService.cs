namespace LashStudio.Application.Common.Abstractions
{
    public interface IJwtTokenService
    {
        string CreateAccessToken(long userId, string email, IEnumerable<string> roles);
    }
}
