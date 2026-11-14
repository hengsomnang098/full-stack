namespace FullStack.Domain.Entities;

public class Supplier
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}
