namespace FullStack.Domain.Entities;

public class Invoice
{
    public int Id { get; set; }
    public int? CustomerId { get; set; }
    public int? EmployeeId { get; set; }
    public int? OrderStatusId { get; set; }
    public int? OrderPaymentMethodId { get; set; }
    public decimal? TotalQty { get; set; } = 0;
    public decimal? TotalAmount { get; set; } = 0;
    public decimal? TotalPaid { get; set; } = 0;
    public string? Note { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }

    public Customer? Customer { get; set; }
    public Employee? Employee { get; set; }
    public OrderStatus? OrderStatus { get; set; }
    public OrderPaymentMethod? OrderPaymentMethod { get; set; }
    public ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}
