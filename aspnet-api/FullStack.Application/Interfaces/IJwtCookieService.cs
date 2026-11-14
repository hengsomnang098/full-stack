namespace FullStack.Application.Interfaces;

public interface IJwtCookieService
{
    void AppendAccessTokenCookie(string token);
    void AppendRefreshTokenCookie(string token);
    void DeleteAuthCookies();
    string? GetAccessToken();
    string? GetRefreshToken();
}
