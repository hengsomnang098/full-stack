using FullStack.Application.Common;
using FullStack.Application.Interfaces;

namespace FullStack.Infrastructure.Auth;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _accessor;

    public CurrentUserService(IHttpContextAccessor accessor) => _accessor = accessor;

    public int? EmployeeId =>
        int.TryParse(_accessor.HttpContext?.User.FindFirst(JwtClaimNames.EmployeeId)?.Value, out var id)
            ? id
            : null;
}
