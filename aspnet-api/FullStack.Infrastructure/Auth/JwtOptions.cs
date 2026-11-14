namespace FullStack.Infrastructure.Auth;

public class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string AccessSecret { get; set; } = string.Empty;
    public string RefreshSecret { get; set; } = string.Empty;
    public int AccessTokenMinutes { get; set; } = 15;
    public int RefreshTokenDays { get; set; } = 7;
    public string AccessCookieName { get; set; } = "access_token";
    public string RefreshCookieName { get; set; } = "refresh_token";
}
