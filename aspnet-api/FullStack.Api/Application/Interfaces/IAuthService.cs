using FullStack.Api.Application.DTOs.Auth;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Application.Interfaces;

public interface IAuthService
{
    Task<ServiceResult<LoginResponse>> LoginAsync(LoginRequest request, CancellationToken ct = default);
    Task<ServiceResult<LoginResponse>> RefreshTokenAsync(CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> SetPasswordAsync(SetPasswordRequest request, CancellationToken ct = default);
    ApiMessageResponse Logout();
    Task<ServiceResult<AuthUserDto>> GetCurrentUserAsync(CancellationToken ct = default);
}
