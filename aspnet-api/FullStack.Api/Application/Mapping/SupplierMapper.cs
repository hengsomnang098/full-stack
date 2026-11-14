using FullStack.Api.Application.DTOs.Suppliers;
using FullStack.Api.Domain.Entities;

namespace FullStack.Api.Application.Mapping;

public static class SupplierMapper
{
    public static SupplierDto ToDto(Supplier e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Tel = e.Tel,
        Email = e.Email,
        Address = e.Address,
        WebsiteUrl = e.WebsiteUrl,
        Status = e.Status,
        CreateAt = e.CreateAt,
        CreateBy = e.CreateBy,
        UpdateAt = e.UpdateAt,
        UpdateBy = e.UpdateBy
    };

    public static Supplier ToEntity(SupplierRequest r, int? employeeId) => new()
    {
        Name = r.Name!.Trim(),
        Tel = r.Tel!.Trim(),
        Email = r.Email!.Trim(),
        Address = r.Address?.Trim() ?? string.Empty,
        WebsiteUrl = r.WebsiteUrl,
        Status = r.Status ?? true,
        CreateBy = employeeId
    };

    public static void Apply(Supplier e, SupplierRequest r, int? employeeId)
    {
        e.Name = r.Name!.Trim();
        e.Tel = r.Tel!.Trim();
        e.Email = r.Email!.Trim();
        e.Address = r.Address?.Trim() ?? string.Empty;
        e.WebsiteUrl = r.WebsiteUrl;
        e.Status = r.Status ?? e.Status;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = employeeId;
    }
}
