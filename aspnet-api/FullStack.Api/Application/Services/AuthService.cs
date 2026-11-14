using FullStack.Api.Application.DTOs.Auth;
using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.Interfaces;
using FullStack.Api.Application.Mapping;
using FullStack.Api.Domain.Entities;
using FullStack.Api.Infrastructure.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IJwtTokenService _jwt;
    private readonly IJwtCookieService _cookies;
    private readonly IHttpContextAccessor _accessor;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IJwtTokenService jwt,
        IJwtCookieService cookies,
        IHttpContextAccessor accessor,
        ILogger<AuthService> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwt = jwt;
        _cookies = cookies;
        _accessor = accessor;
        _logger = logger;
    }

    public async Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
            return ServiceResult<LoginResponse>.Fail("Username and password are required.", 400);

        var tel = request.Username.Trim();
        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Tel == tel, ct);
        if (user is null)
        {
            _logger.LogWarning("Login failed: unknown tel {Tel}", tel);
            return ServiceResult<LoginResponse>.Fail("Invalid username or password.", 401);
        }

        if (!user.Status)
            return ServiceResult<LoginResponse>.Fail("Account is disabled.", 403);

        var check = await _signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (check.IsLockedOut)
            return ServiceResult<LoginResponse>.Fail("Account is locked. Try again later.", 423);
        if (!check.Succeeded)
        {
            _logger.LogWarning("Login failed: wrong password for {Tel}", tel);
            return ServiceResult<LoginResponse>.Fail("Invalid username or password.", 401);
        }

        var roles = await _userManager.GetRolesAsync(user);
        IssueCookies(user, roles);
        _logger.LogInformation("User {UserId} logged in", user.Id);

        return ServiceResult<LoginResponse>.Ok(AuthMapper.ToLoginResponse(user, roles, "Login success"));
    }

    public async Task<ServiceResult<LoginResponse>> RefreshTokenAsync(CancellationToken ct = default)
    {
        var token = _cookies.GetRefreshToken();
        if (string.IsNullOrWhiteSpace(token))
            return ServiceResult<LoginResponse>.Fail("Refresh token is missing.", 401);

        var principal = _jwt.ValidateRefreshToken(token);
        var userId = principal?.FindFirst("sub")?.Value;
        if (principal is null || string.IsNullOrEmpty(userId))
        {
            _cookies.DeleteAuthCookies();
            return ServiceResult<LoginResponse>.Fail("Invalid refresh token.", 401);
        }

        var user = await _userManager.FindByIdAsync(userId);
        var stamp = principal.FindFirst(JwtTokenService.StampClaim)?.Value;
        if (user is null || !user.Status || stamp != user.SecurityStamp)
        {
            _cookies.DeleteAuthCookies();
            return ServiceResult<LoginResponse>.Fail("Invalid refresh token.", 401);
        }

        var roles = await _userManager.GetRolesAsync(user);
        IssueCookies(user, roles);

        return ServiceResult<LoginResponse>.Ok(AuthMapper.ToLoginResponse(user, roles, "Token refreshed"));
    }

    public async Task<ServiceResult<ApiMessageResponse>> SetPasswordAsync(SetPasswordRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.NewPassword))
            return ServiceResult<ApiMessageResponse>.Fail("New password is required.", 400);

        var user = await GetCurrentApplicationUserAsync();
        if (user is null)
            return ServiceResult<ApiMessageResponse>.Fail("Unauthorized.", 401);

        IdentityResult result;
        if (await _userManager.HasPasswordAsync(user))
        {
            if (string.IsNullOrWhiteSpace(request.CurrentPassword))
                return ServiceResult<ApiMessageResponse>.Fail("Current password is required.", 400);
            result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        }
        else
        {
            result = await _userManager.AddPasswordAsync(user, request.NewPassword);
        }

        if (!result.Succeeded)
            return ServiceResult<ApiMessageResponse>.Fail(string.Join(" ", result.Errors.Select(e => e.Description)), 400);

        // Security stamp changed => reissue cookies for the current session.
        var roles = await _userManager.GetRolesAsync(user);
        IssueCookies(user, roles);

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Password updated"));
    }

    public ApiMessageResponse Logout()
    {
        _cookies.DeleteAuthCookies();
        return new ApiMessageResponse("Logout success");
    }

    public async Task<ServiceResult<AuthUserDto>> GetCurrentUserAsync(CancellationToken ct = default)
    {
        var user = await GetCurrentApplicationUserAsync();
        if (user is null)
            return ServiceResult<AuthUserDto>.Fail("Unauthorized.", 401);

        var roles = await _userManager.GetRolesAsync(user);
        return ServiceResult<AuthUserDto>.Ok(AuthMapper.ToDto(user, roles));
    }

    private async Task<ApplicationUser?> GetCurrentApplicationUserAsync()
    {
        var id = _accessor.HttpContext?.User.FindFirst("sub")?.Value;
        return string.IsNullOrEmpty(id) ? null : await _userManager.FindByIdAsync(id);
    }

    private void IssueCookies(ApplicationUser user, IEnumerable<string> roles)
    {
        _cookies.AppendAccessTokenCookie(_jwt.CreateAccessToken(user, roles));
        _cookies.AppendRefreshTokenCookie(_jwt.CreateRefreshToken(user));
    }
}
