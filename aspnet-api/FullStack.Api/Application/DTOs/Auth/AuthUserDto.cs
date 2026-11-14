using System.Text.Json.Serialization;

namespace FullStack.Api.Application.DTOs.Auth;

public class AuthUserDto
{
    public string Id { get; set; } = string.Empty;
    public int? EmployeeId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;

    [JsonPropertyName("Firstname")]
    public string FirstName { get; set; } = string.Empty;

    [JsonPropertyName("Lastname")]
    public string LastName { get; set; } = string.Empty;

    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public bool? Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string? Address { get; set; }
    public bool Status { get; set; }
    public string? Image { get; set; }
    public decimal? Salary { get; set; }
    public IReadOnlyList<string> Roles { get; set; } = Array.Empty<string>();
}
