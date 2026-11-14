namespace FullStack.Domain.Entities;

public class Product
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }

    /// <summary>Not present in the original SQL dump; added as a column "Name".</summary>
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Qty { get; set; } = 0;
    public decimal? Price { get; set; } = 0;
    public decimal? Discount { get; set; } = 0;
    public string? Image { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }

    public Category? Category { get; set; }
}
