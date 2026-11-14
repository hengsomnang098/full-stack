using System.Text.Json.Serialization;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Application.DTOs.Invoices;

/// <summary>Row of GET /api/invoice (same columns as the Express query).</summary>
public class InvoiceListItemDto
{
    public int Id { get; set; }
    public decimal? TotalQty { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? TotalPaid { get; set; }
    public string? Note { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public DateTime CreateAt { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }
    public string? CustomerName { get; set; }
    public string? EmployeeName { get; set; }
    public string? PaymentMethod { get; set; }
    public string? OrderStatus { get; set; }
}

/// <summary>Row of GET /api/invoice_details/{Id}.</summary>
public class InvoiceDetailDto
{
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public int? Qty { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Name { get; set; }
    public string? Image { get; set; }
}
