namespace FullStack.Api.Domain.Entities;

/// <summary>Maps to table invoice_details.</summary>
public class InvoiceDetail
{
    public int Id { get; set; }
    public int? InvoiceId { get; set; }
    public int? ProductId { get; set; }
    public int? Qty { get; set; } = 1;
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;

    public Invoice? Invoice { get; set; }
    public Product? Product { get; set; }
}
