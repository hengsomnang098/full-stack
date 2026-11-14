using System.Security.Claims;
using FullStack.Api.Domain.Entities;

namespace FullStack.Api.Infrastructure.Auth;

public interface IJwtTokenService
{
    string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles);
    string CreateRefreshToken(ApplicationUser user);

    /// <summary>Returns the principal when the refresh token is valid; otherwise null.</summary>
    ClaimsPrincipal? ValidateRefreshToken(string token);
}
