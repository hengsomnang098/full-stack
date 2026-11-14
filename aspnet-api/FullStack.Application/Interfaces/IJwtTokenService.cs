using System.Security.Claims;
using FullStack.Domain.Entities;

namespace FullStack.Application.Interfaces;

public interface IJwtTokenService
{
    string CreateAccessToken(ApplicationUser user, IEnumerable<string> roles);
    string CreateRefreshToken(ApplicationUser user);

    /// <summary>Returns the principal when the refresh token is valid; otherwise null.</summary>
    ClaimsPrincipal? ValidateRefreshToken(string token);
}
