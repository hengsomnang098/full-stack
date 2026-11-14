using FullStack.Api.Application.DTOs.Roles;
using Microsoft.AspNetCore.Identity;

namespace FullStack.Api.Application.Mapping;

public static class RoleMapper
{
    public static RoleDto ToDto(IdentityRole role) => new()
    {
        Id = role.Id,
        Name = role.Name ?? string.Empty,
        Code = role.Name ?? string.Empty,
        Status = true
    };

    public static string ResolveName(RoleRequest request)
    {
        if (!string.IsNullOrWhiteSpace(request.Name)) return request.Name.Trim();
        if (!string.IsNullOrWhiteSpace(request.Code)) return request.Code.Trim();
        return string.Empty;
    }
}
