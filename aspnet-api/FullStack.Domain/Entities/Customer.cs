namespace FullStack.Domain.Entities;

public class Customer
{
    public int Id { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;
    public bool? Gender { get; set; } = true;
    public DateTime? Dob { get; set; }
    public string Tel { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }
    public bool? Status { get; set; } = true;
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }

    public ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
