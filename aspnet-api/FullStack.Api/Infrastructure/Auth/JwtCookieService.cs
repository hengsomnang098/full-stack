using Microsoft.Extensions.Options;

namespace FullStack.Api.Infrastructure.Auth;

public class JwtCookieService : IJwtCookieService
{
    private readonly IHttpContextAccessor _accessor;
    private readonly JwtOptions _options;

    public JwtCookieService(IHttpContextAccessor accessor, IOptions<JwtOptions> options)
    {
        _accessor = accessor;
        _options = options.Value;
    }

    private HttpContext Context =>
        _accessor.HttpContext ?? throw new InvalidOperationException("No active HttpContext.");

    public void AppendAccessTokenCookie(string token) =>
        Context.Response.Cookies.Append(_options.AccessCookieName, token,
            BuildOptions(DateTimeOffset.UtcNow.AddMinutes(_options.AccessTokenMinutes)));

    public void AppendRefreshTokenCookie(string token) =>
        Context.Response.Cookies.Append(_options.RefreshCookieName, token,
            BuildOptions(DateTimeOffset.UtcNow.AddDays(_options.RefreshTokenDays)));

    public void DeleteAuthCookies()
    {
        var opts = BuildOptions(null);
        Context.Response.Cookies.Delete(_options.AccessCookieName, opts);
        Context.Response.Cookies.Delete(_options.RefreshCookieName, opts);
    }

    public string? GetAccessToken() =>
        Context.Request.Cookies.TryGetValue(_options.AccessCookieName, out var v) ? v : null;

    public string? GetRefreshToken() =>
        Context.Request.Cookies.TryGetValue(_options.RefreshCookieName, out var v) ? v : null;

    private CookieOptions BuildOptions(DateTimeOffset? expires) => new()
    {
        HttpOnly = true,
        Secure = Context.Request.IsHttps,
        SameSite = SameSiteMode.Lax,
        Path = "/",
        Expires = expires,
        IsEssential = true
    };
}
