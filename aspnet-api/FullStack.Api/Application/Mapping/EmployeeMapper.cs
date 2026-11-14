using FullStack.Api.Application.Common;
using FullStack.Api.Application.DTOs.Employees;
using FullStack.Api.Domain.Entities;

namespace FullStack.Api.Application.Mapping;

public static class EmployeeMapper
{
    public static EmployeeDto ToDto(Employee e, string? roleId = null, string? roleName = null) => new()
    {
        Id = e.Id,
        RoleId = roleId,
        RoleName = roleName,
        Firstname = e.Firstname,
        Lastname = e.Lastname,
        Gender = e.Gender,
        Dob = e.Dob,
        Tel = e.Tel,
        Email = e.Email,
        Address = e.Address,
        Status = e.Status,
        Image = e.Image,
        Salary = e.Salary,
        CreateAt = e.CreateAt,
        CreateBy = e.CreateBy,
        UpdateAt = e.UpdateAt,
        UpdateBy = e.UpdateBy
    };

    public static Employee ToEntity(EmployeeRequest r, int? createdBy) => new()
    {
        Firstname = r.Firstname!.Trim(),
        Lastname = r.Lastname!.Trim(),
        Gender = r.Gender ?? true,
        Dob = r.Dob,
        Tel = r.Tel!.Trim(),
        Email = LegacyParse.Text(r.Email),
        Address = r.Address,
        Status = r.Status ?? true,
        Salary = r.Salary,
        CreateBy = createdBy
    };

    public static void Apply(Employee e, EmployeeRequest r, int? updatedBy)
    {
        e.Firstname = r.Firstname!.Trim();
        e.Lastname = r.Lastname!.Trim();
        e.Gender = r.Gender ?? e.Gender;
        e.Dob = r.Dob;
        e.Tel = r.Tel!.Trim();
        e.Email = LegacyParse.Text(r.Email);
        e.Address = r.Address;
        e.Status = r.Status ?? e.Status;
        e.Salary = r.Salary;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = updatedBy;
    }

    public static ApplicationUser ToApplicationUser(Employee e) => new()
    {
        UserName = e.Tel,
        Tel = e.Tel,
        EmployeeId = e.Id,
        Email = e.Email,
        EmailConfirmed = e.Email is not null
    };

    public static void SyncToUser(ApplicationUser u, Employee e)
    {
        u.FirstName = e.Firstname;
        u.LastName = e.Lastname;
        u.Gender = e.Gender;
        u.Dob = e.Dob;
        u.Tel = e.Tel;
        u.Address = e.Address;
        u.Status = e.Status ?? true;
        u.Image = e.Image;
        u.Salary = e.Salary;
        u.Email = e.Email;
    }
}
