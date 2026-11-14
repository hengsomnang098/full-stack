using FullStack.Application.DTOs.Customers;
using FullStack.Domain.Entities;

namespace FullStack.Application.Mapping;

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer e) => new()
    {
        Id = e.Id,
        Firstname = e.Firstname,
        Lastname = e.Lastname,
        Gender = e.Gender,
        Dob = e.Dob,
        Tel = e.Tel,
        Email = e.Email,
        Address = e.Address,
        Status = e.Status,
        CreateAt = e.CreateAt,
        CreateBy = e.CreateBy,
        UpdateAt = e.UpdateAt,
        UpdateBy = e.UpdateBy
    };

    public static Customer ToEntity(CustomerRequest r, int? employeeId) => new()
    {
        Firstname = r.Firstname!.Trim(),
        Lastname = r.Lastname!.Trim(),
        Gender = r.Gender ?? true,
        Dob = r.Dob,
        Tel = r.Tel!.Trim(),
        Email = r.Email,
        Address = r.Address,
        Status = r.Status ?? true,
        CreateBy = employeeId
    };

    public static void Apply(Customer e, CustomerRequest r, int? employeeId)
    {
        e.Firstname = r.Firstname!.Trim();
        e.Lastname = r.Lastname!.Trim();
        e.Gender = r.Gender ?? e.Gender;
        e.Dob = r.Dob;
        e.Tel = r.Tel!.Trim();
        e.Email = r.Email;
        e.Address = r.Address;
        e.Status = r.Status ?? e.Status;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = employeeId;
    }
}
