using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Roles;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

/// <summary>Role CRUD backed by ASP.NET Identity <see cref="IdentityRole"/>.</summary>
public class RoleService : IRoleService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<Domain.Entities.ApplicationUser> _userManager;
    private readonly ILogger<RoleService> _logger;

    public RoleService(
        RoleManager<IdentityRole> roleManager,
        UserManager<Domain.Entities.ApplicationUser> userManager,
        ILogger<RoleService> logger)
    {
        _roleManager = roleManager;
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<ServiceResult<ListResponse<RoleDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default)
    {
        var query = _roleManager.Roles.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(txtSearch))
        {
            var s = txtSearch.Trim();
            query = query.Where(r => r.Name != null && r.Name.Contains(s));
        }

        // Identity roles have no Status column; inactive filter returns empty.
        if (status == "0" || string.Equals(status, "false", StringComparison.OrdinalIgnoreCase))
            return ServiceResult<ListResponse<RoleDto>>.Ok(ListResponse<RoleDto>.From(Array.Empty<RoleDto>()));

        var items = await query.OrderBy(r => r.Name).ToListAsync(ct);
        return ServiceResult<ListResponse<RoleDto>>.Ok(
            ListResponse<RoleDto>.From(items.Select(RoleMapper.ToDto).ToList()));
    }

    public async Task<ServiceResult<MessageDataResponse<RoleDto>>> CreateAsync(RoleRequest request, CancellationToken ct = default)
    {
        var name = RoleMapper.ResolveName(request);
        if (string.IsNullOrWhiteSpace(name))
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("Name is required.", 400);

        if (await _roleManager.RoleExistsAsync(name))
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("A role with the same name already exists.", 409);

        var role = new IdentityRole(name);
        var result = await _roleManager.CreateAsync(role);
        if (!result.Succeeded)
        {
            _logger.LogWarning("Create role failed: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail(
                string.Join(" ", result.Errors.Select(e => e.Description)), 400);
        }

        return ServiceResult<MessageDataResponse<RoleDto>>.Ok(
            MessageDataResponse<RoleDto>.From("Insert success", RoleMapper.ToDto(role)));
    }

    public async Task<ServiceResult<MessageDataResponse<RoleDto>>> UpdateAsync(RoleRequest request, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(request.Id))
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("Id is required.", 400);

        var role = await _roleManager.FindByIdAsync(request.Id);
        if (role is null)
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("Not found", 404);

        var name = RoleMapper.ResolveName(request);
        if (string.IsNullOrWhiteSpace(name))
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("Name is required.", 400);

        if (!string.Equals(role.Name, name, StringComparison.OrdinalIgnoreCase)
            && await _roleManager.RoleExistsAsync(name))
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail("A role with the same name already exists.", 409);

        var setName = await _roleManager.SetRoleNameAsync(role, name);
        if (!setName.Succeeded)
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail(
                string.Join(" ", setName.Errors.Select(e => e.Description)), 400);

        var update = await _roleManager.UpdateAsync(role);
        if (!update.Succeeded)
            return ServiceResult<MessageDataResponse<RoleDto>>.Fail(
                string.Join(" ", update.Errors.Select(e => e.Description)), 400);

        return ServiceResult<MessageDataResponse<RoleDto>>.Ok(
            MessageDataResponse<RoleDto>.From("Update success", RoleMapper.ToDto(role)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(string id, CancellationToken ct = default)
    {
        var role = await _roleManager.FindByIdAsync(id);
        if (role is null)
            return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        if (string.Equals(role.Name, "Admin", StringComparison.OrdinalIgnoreCase))
            return ServiceResult<ApiMessageResponse>.Fail("The Admin role cannot be deleted.", 400);

        var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name!);
        if (usersInRole.Count > 0)
            return ServiceResult<ApiMessageResponse>.Fail("Role is assigned to users and cannot be deleted.", 409);

        var result = await _roleManager.DeleteAsync(role);
        if (!result.Succeeded)
            return ServiceResult<ApiMessageResponse>.Fail(
                string.Join(" ", result.Errors.Select(e => e.Description)), 400);

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }
}
