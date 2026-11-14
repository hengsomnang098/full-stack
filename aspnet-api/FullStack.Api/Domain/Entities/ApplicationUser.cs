using Microsoft.AspNetCore.Identity;

namespace FullStack.Api.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public bool? Gender { get; set; } = true;
    public DateTime? Dob { get; set; }
    public string Tel { get; set; } = string.Empty;
    public string? Address { get; set; }
    public bool Status { get; set; } = true;
    public string? Image { get; set; }
    public decimal? Salary { get; set; }

    /// <summary>Optional link to the legacy business employee row.</summary>
    public int? EmployeeId { get; set; }
}
