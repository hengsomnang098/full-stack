namespace FullStack.Api.Infrastructure.Auth;

public interface IJwtCookieService
{
    void AppendAccessTokenCookie(string token);
    void AppendRefreshTokenCookie(string token);
    void DeleteAuthCookies();
    string? GetAccessToken();
    string? GetRefreshToken();
}
