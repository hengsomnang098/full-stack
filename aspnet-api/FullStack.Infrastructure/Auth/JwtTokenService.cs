using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FullStack.Application.Common;
using FullStack.Application.Interfaces;
using FullStack.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace FullStack.Infrastructure.Auth;

public class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _handler = new() { MapInboundClaims = false };

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtClaimNames.Tel, user.Tel ?? string.Empty),
            new("name", $"{user.FirstName} {user.LastName}".Trim()),
            new(JwtClaimNames.TokenType, "access")
        };

        if (user.EmployeeId.HasValue)
            claims.Add(new Claim(JwtClaimNames.EmployeeId, user.EmployeeId.Value.ToString()));

        claims.AddRange(roles.Select(r => new Claim("role", r)));

        return WriteToken(claims, _options.AccessSecret, DateTime.UtcNow.AddMinutes(_options.AccessTokenMinutes));
    }

    public string CreateRefreshToken(ApplicationUser user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
            new(JwtClaimNames.Stamp, user.SecurityStamp ?? string.Empty),
            new(JwtClaimNames.TokenType, "refresh")
        };

        return WriteToken(claims, _options.RefreshSecret, DateTime.UtcNow.AddDays(_options.RefreshTokenDays));
    }

    public ClaimsPrincipal? ValidateRefreshToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token)) return null;

        var parameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _options.Issuer,
            ValidateAudience = true,
            ValidAudience = _options.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.RefreshSecret)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };

        try
        {
            var principal = _handler.ValidateToken(token, parameters, out _);
            return principal.FindFirst(JwtClaimNames.TokenType)?.Value == "refresh" ? principal : null;
        }
        catch (Exception ex) when (ex is SecurityTokenException or ArgumentException)
        {
            return null;
        }
    }

    private string WriteToken(IEnumerable<Claim> claims, string secret, DateTime expiresUtc)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expiresUtc,
            signingCredentials: creds);
        return _handler.WriteToken(token);
    }
}
