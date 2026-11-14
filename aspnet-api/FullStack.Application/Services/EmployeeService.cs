using System.Security.Cryptography;
using FullStack.Application.Common;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Employees;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Domain.Entities;
using FullStack.Application.Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IAppDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<EmployeeService> _logger;

    public EmployeeService(
        IAppDbContext db,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ICurrentUserService currentUser,
        ILogger<EmployeeService> logger)
    {
        _db = db;
        _userManager = userManager;
        _roleManager = roleManager;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<EmployeeListResponse>> GetListAsync(string? txtSearch, string? status, string? roleId, CancellationToken ct = default)
    {
        var query = _db.Employees.AsNoTracking().AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(e => e.Firstname.Contains(search) || e.Lastname.Contains(search));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(e => e.Status == statusFilter.Value);

        if (!LegacyParse.IsEmpty(roleId) && !LegacyParse.IsAll(roleId))
        {
            var role = await _roleManager.FindByIdAsync(roleId!.Trim());
            if (role?.Name is null)
                return ServiceResult<EmployeeListResponse>.Ok(new EmployeeListResponse
                {
                    List = Array.Empty<EmployeeDto>(),
                    Role = await GetIdentityRolesAsync(ct)
                });

            var usersInRole = await _userManager.GetUsersInRoleAsync(role.Name);
            var employeeIds = usersInRole
                .Where(u => u.EmployeeId.HasValue)
                .Select(u => u.EmployeeId!.Value)
                .ToHashSet();
            query = query.Where(e => employeeIds.Contains(e.Id));
        }

        var items = await query.OrderByDescending(e => e.Id).ToListAsync(ct);
        var roleLookup = await BuildEmployeeRoleLookupAsync(items.Select(e => e.Id).ToList(), ct);
        var roles = await GetIdentityRolesAsync(ct);

        return ServiceResult<EmployeeListResponse>.Ok(new EmployeeListResponse
        {
            List = items.Select(e =>
            {
                roleLookup.TryGetValue(e.Id, out var info);
                return EmployeeMapper.ToDto(e, info.RoleId, info.RoleName);
            }).ToList(),
            Role = roles
        });
    }

    public async Task<ServiceResult<DataResponse<EmployeeDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Employees.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, ct);
        if (entity is null)
            return ServiceResult<DataResponse<EmployeeDto>>.Fail("Not found", 404);

        var (roleId, roleName) = await GetEmployeeRoleAsync(id, ct);
        return ServiceResult<DataResponse<EmployeeDto>>.Ok(
            DataResponse<EmployeeDto>.From(EmployeeMapper.ToDto(entity, roleId, roleName)));
    }

    public async Task<ServiceResult<EmployeeCreateResponse>> CreateAsync(EmployeeRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<EmployeeCreateResponse>.FailValidation(errors);

        var tel = request.Tel!.Trim();
        if (await TelInUseAsync(tel, excludeEmployeeId: null, ct))
            return ServiceResult<EmployeeCreateResponse>.Fail("An employee with this Tel already exists.", 409);

        IdentityRole? identityRole = null;
        if (!string.IsNullOrWhiteSpace(request.RoleId))
        {
            identityRole = await _roleManager.FindByIdAsync(request.RoleId.Trim());
            if (identityRole is null)
                return ServiceResult<EmployeeCreateResponse>.Fail("RoleId does not exist.", 400);
        }

        var generated = string.IsNullOrWhiteSpace(request.Password);
        var password = generated ? GenerateTemporaryPassword() : request.Password!;

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        var employee = EmployeeMapper.ToEntity(request, _currentUser.EmployeeId);
        _db.Employees.Add(employee);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Employee create failed");
            return ServiceResult<EmployeeCreateResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        var user = EmployeeMapper.ToApplicationUser(employee);
        EmployeeMapper.SyncToUser(user, employee);

        var created = await _userManager.CreateAsync(user, password);
        if (!created.Succeeded)
        {
            await tx.RollbackAsync(ct);
            _logger.LogWarning("Identity user creation failed for tel {Tel}: {Errors}", tel,
                string.Join(", ", created.Errors.Select(e => e.Code)));
            return ServiceResult<EmployeeCreateResponse>.Fail(
                string.Join(" ", created.Errors.Select(e => e.Description)), 400);
        }

        if (identityRole?.Name is not null)
        {
            var addRole = await _userManager.AddToRoleAsync(user, identityRole.Name);
            if (!addRole.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return ServiceResult<EmployeeCreateResponse>.Fail(
                    string.Join(" ", addRole.Errors.Select(e => e.Description)), 400);
            }
        }

        await tx.CommitAsync(ct);

        return ServiceResult<EmployeeCreateResponse>.Ok(new EmployeeCreateResponse
        {
            Message = "Create success",
            Data = EmployeeMapper.ToDto(employee, identityRole?.Id, identityRole?.Name),
            TemporaryPassword = generated ? password : null
        });
    }

    public async Task<ServiceResult<MessageDataResponse<EmployeeDto>>> UpdateAsync(EmployeeRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<EmployeeDto>>.FailValidation(errors);

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == request.Id, ct);
        if (employee is null) return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail("Not found", 404);

        var tel = request.Tel!.Trim();
        if (await TelInUseAsync(tel, excludeEmployeeId: employee.Id, ct))
            return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail("An employee with this Tel already exists.", 409);

        IdentityRole? identityRole = null;
        if (!string.IsNullOrWhiteSpace(request.RoleId))
        {
            identityRole = await _roleManager.FindByIdAsync(request.RoleId.Trim());
            if (identityRole is null)
                return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail("RoleId does not exist.", 400);
        }

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        EmployeeMapper.Apply(employee, request, _currentUser.EmployeeId);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Employee update failed");
            return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        var user = await _userManager.Users.FirstOrDefaultAsync(u => u.EmployeeId == employee.Id, ct);
        if (user is not null)
        {
            var telChanged = !string.Equals(user.UserName, employee.Tel, StringComparison.Ordinal);
            EmployeeMapper.SyncToUser(user, employee);

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded && telChanged)
                result = await _userManager.SetUserNameAsync(user, employee.Tel);

            if (!result.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail(
                    string.Join(" ", result.Errors.Select(e => e.Description)), 400);
            }

            var roleResult = await ReplaceUserRolesAsync(user, identityRole?.Name, ct);
            if (!roleResult.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return ServiceResult<MessageDataResponse<EmployeeDto>>.Fail(
                    string.Join(" ", roleResult.Errors.Select(e => e.Description)), 400);
            }
        }

        await tx.CommitAsync(ct);

        return ServiceResult<MessageDataResponse<EmployeeDto>>.Ok(
            MessageDataResponse<EmployeeDto>.From(
                "Update success",
                EmployeeMapper.ToDto(employee, identityRole?.Id, identityRole?.Name)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        if (_currentUser.EmployeeId == id)
            return ServiceResult<ApiMessageResponse>.Fail("You cannot delete your own account.", 400);

        var employee = await _db.Employees.FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        _db.Employees.Remove(employee);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Employee delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        var users = await _userManager.Users.Where(u => u.EmployeeId == id).ToListAsync(ct);
        foreach (var user in users)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                await tx.RollbackAsync(ct);
                return ServiceResult<ApiMessageResponse>.Fail(
                    string.Join(" ", result.Errors.Select(e => e.Description)), 400);
            }
        }

        await tx.CommitAsync(ct);
        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private async Task<IdentityResult> ReplaceUserRolesAsync(ApplicationUser user, string? newRoleName, CancellationToken ct)
    {
        var current = await _userManager.GetRolesAsync(user);
        if (current.Count > 0)
        {
            var remove = await _userManager.RemoveFromRolesAsync(user, current);
            if (!remove.Succeeded) return remove;
        }

        if (string.IsNullOrWhiteSpace(newRoleName))
            return IdentityResult.Success;

        return await _userManager.AddToRoleAsync(user, newRoleName);
    }

    private async Task<List<DTOs.Roles.RoleDto>> GetIdentityRolesAsync(CancellationToken ct) =>
        (await _roleManager.Roles.AsNoTracking().OrderBy(r => r.Name).ToListAsync(ct))
        .Select(RoleMapper.ToDto)
        .ToList();

    private async Task<(string? RoleId, string? RoleName)> GetEmployeeRoleAsync(int employeeId, CancellationToken ct)
    {
        var user = await _userManager.Users.AsNoTracking()
            .FirstOrDefaultAsync(u => u.EmployeeId == employeeId, ct);
        if (user is null) return (null, null);

        var roles = await _userManager.GetRolesAsync(user);
        var name = roles.FirstOrDefault();
        if (name is null) return (null, null);

        var role = await _roleManager.FindByNameAsync(name);
        return (role?.Id, name);
    }

    private async Task<Dictionary<int, (string? RoleId, string? RoleName)>> BuildEmployeeRoleLookupAsync(
        IReadOnlyList<int> employeeIds, CancellationToken ct)
    {
        var result = new Dictionary<int, (string? RoleId, string? RoleName)>();
        if (employeeIds.Count == 0) return result;

        var users = await _userManager.Users.AsNoTracking()
            .Where(u => u.EmployeeId != null && employeeIds.Contains(u.EmployeeId.Value))
            .ToListAsync(ct);

        var allRoles = await _roleManager.Roles.AsNoTracking().ToListAsync(ct);
        var rolesByName = allRoles
            .Where(r => r.Name != null)
            .ToDictionary(r => r.Name!, r => r, StringComparer.OrdinalIgnoreCase);

        foreach (var user in users)
        {
            if (!user.EmployeeId.HasValue) continue;
            var names = await _userManager.GetRolesAsync(user);
            var name = names.FirstOrDefault();
            if (name is null) continue;
            rolesByName.TryGetValue(name, out var role);
            result[user.EmployeeId.Value] = (role?.Id, name);
        }

        return result;
    }

    private async Task<bool> TelInUseAsync(string tel, int? excludeEmployeeId, CancellationToken ct)
    {
        var employees = _db.Employees.Where(e => e.Tel == tel);
        var users = _userManager.Users.Where(u => u.Tel == tel || u.UserName == tel);

        if (excludeEmployeeId is { } excluded)
        {
            employees = employees.Where(e => e.Id != excluded);
            users = users.Where(u => u.EmployeeId == null || u.EmployeeId != excluded);
        }

        return await employees.AnyAsync(ct) || await users.AnyAsync(ct);
    }

    private static Dictionary<string, string> Validate(EmployeeRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Firstname)) errors["Firstname"] = "Firstname required!";
        if (string.IsNullOrWhiteSpace(r.Lastname)) errors["Lastname"] = "Lastname required!";
        if (string.IsNullOrWhiteSpace(r.Tel)) errors["Tel"] = "Tel required!";
        return errors;
    }

    private static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnpqrstuvwxyz";
        const string digits = "23456789";
        const string symbols = "!@#$%^&*?";
        var all = upper + lower + digits + symbols;

        var chars = new List<char>
        {
            Pick(upper), Pick(lower), Pick(digits), Pick(symbols)
        };
        while (chars.Count < 16) chars.Add(Pick(all));

        for (var i = chars.Count - 1; i > 0; i--)
        {
            var j = RandomNumberGenerator.GetInt32(i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }
        return new string(chars.ToArray());

        static char Pick(string set) => set[RandomNumberGenerator.GetInt32(set.Length)];
    }
}
