namespace FullStack.Domain.Entities;

public class OrderStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public bool? Status { get; set; } = true;
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}
