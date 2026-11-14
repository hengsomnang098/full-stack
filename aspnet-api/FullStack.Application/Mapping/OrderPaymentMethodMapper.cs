using FullStack.Application.DTOs.PaymentMethods;
using FullStack.Domain.Entities;

namespace FullStack.Application.Mapping;

public static class OrderPaymentMethodMapper
{
    public static OrderPaymentMethodDto ToDto(OrderPaymentMethod e) => new()
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

    public static OrderPaymentMethod ToEntity(OrderPaymentMethodRequest r, int? employeeId) => new()
    {
        Name = r.Name!.Trim(),
        Code = r.Code!.Trim(),
        Status = r.Status ?? true,
        CreateBy = employeeId
    };

    public static void Apply(OrderPaymentMethod e, OrderPaymentMethodRequest r, int? employeeId)
    {
        e.Name = r.Name!.Trim();
        e.Code = r.Code!.Trim();
        e.Status = r.Status ?? e.Status;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = employeeId;
    }
}
