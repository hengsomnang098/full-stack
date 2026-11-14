using FullStack.Api.Application.DTOs.OrderStatuses;
using FullStack.Api.Domain.Entities;

namespace FullStack.Api.Application.Mapping;

public static class OrderStatusMapper
{
    public static OrderStatusDto ToDto(OrderStatus e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Code = e.Code,
        Status = e.Status,
        CreateAt = e.CreateAt,
        CreateBy = e.CreateBy,
        UpdateAt = e.UpdateAt,
        UpdateBy = e.UpdateBy
    };

    public static OrderStatus ToEntity(OrderStatusRequest r, int? employeeId) => new()
    {
        Name = r.Name!.Trim(),
        Code = r.Code!.Trim(),
        Status = r.Status ?? true,
        CreateBy = employeeId
    };

    public static void Apply(OrderStatus e, OrderStatusRequest r, int? employeeId)
    {
        e.Name = r.Name!.Trim();
        e.Code = r.Code!.Trim();
        e.Status = r.Status ?? e.Status;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = employeeId;
    }
}
