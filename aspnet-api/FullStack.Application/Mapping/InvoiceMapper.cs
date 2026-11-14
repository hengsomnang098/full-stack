using FullStack.Application.DTOs.Invoices;
using FullStack.Domain.Entities;

namespace FullStack.Application.Mapping;

public static class InvoiceMapper
{
    /// <summary>Expects Customer, Employee, OrderPaymentMethod and OrderStatus to be loaded.</summary>
    public static InvoiceListItemDto ToListItem(Invoice e) => new()
    {
        Id = e.Id,
        TotalQty = e.TotalQty,
        TotalAmount = e.TotalAmount,
        TotalPaid = e.TotalPaid,
        Note = e.Note,
        Status = e.Status,
        CreateAt = e.CreateAt,
        Firstname = e.Customer?.Firstname,
        Lastname = e.Customer?.Lastname,
        CustomerName = e.Customer is null ? null : $"{e.Customer.Firstname}-{e.Customer.Lastname}",
        EmployeeName = e.Employee is null ? null : $"{e.Employee.Firstname}-{e.Employee.Lastname}",
        PaymentMethod = e.OrderPaymentMethod?.Name,
        OrderStatus = e.OrderStatus?.Name
    };

    /// <summary>Expects Product to be loaded.</summary>
    public static InvoiceDetailDto ToDetailDto(InvoiceDetail e) => new()
    {
        Id = e.Id,
        InvoiceId = e.InvoiceId,
        Qty = e.Qty,
        Price = e.Price,
        Discount = e.Discount,
        Name = e.Product?.Name,
        Image = e.Product?.Image
    };
}
