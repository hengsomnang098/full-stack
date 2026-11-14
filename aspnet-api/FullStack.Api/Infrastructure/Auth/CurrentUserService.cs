using FullStack.Api.Application.Interfaces;

namespace FullStack.Api.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public int? EmployeeId =>
        int.TryParse(_accessor.HttpContext?.User.FindFirst(JwtTokenService.EmployeeIdClaim)?.Value, out var id)
            ? id
            : null;
}
