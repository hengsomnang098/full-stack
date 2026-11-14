using FullStack.Application.DTOs.Auth;
using FullStack.Domain.Entities;

namespace FullStack.Application.Mapping;

public static class AuthMapper
{
    public static AuthUserDto ToDto(ApplicationUser user, IEnumerable<string>? roles = null) => new()
    {
        Id = user.Id,
        EmployeeId = user.EmployeeId,
        Username = user.UserName ?? string.Empty,
        Tel = user.Tel,
        FirstName = user.FirstName,
        LastName = user.LastName,
        FullName = $"{user.FirstName} {user.LastName}".Trim(),
        Email = user.Email,
        Gender = user.Gender,
        Dob = user.Dob,
        Address = user.Address,
        Status = user.Status,
        Image = user.Image,
        Salary = user.Salary,
        Roles = (roles ?? Array.Empty<string>()).ToList()
    };

    public static LoginResponse ToLoginResponse(ApplicationUser user, IEnumerable<string> roles, string message) => new()
    {
        Message = message,
        User = ToDto(user, roles)
    };
}
