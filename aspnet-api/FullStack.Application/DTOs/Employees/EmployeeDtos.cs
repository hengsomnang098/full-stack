using System.Text.Json.Serialization;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Roles;

namespace FullStack.Application.DTOs.Employees;

/// <summary>Employee row returned to clients. Never contains any password information.</summary>
public class EmployeeDto
{
    public int Id { get; set; }

    /// <summary>ASP.NET Identity role Id (string).</summary>
    public string? RoleId { get; set; }
    public string? RoleName { get; set; }
    public string Firstname { get; set; } = string.Empty;
    public string Lastname { get; set; } = string.Empty;

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Gender { get; set; }

    [JsonConverter(typeof(LegacyDateTimeConverter))]
    public DateTime? Dob { get; set; }
    public string Tel { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Address { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public string? Image { get; set; }
    public decimal? Salary { get; set; }
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}

public class EmployeeRequest
{
    public int? Id { get; set; }

    /// <summary>ASP.NET Identity role Id (string).</summary>
    public string? RoleId { get; set; }
    public string? Firstname { get; set; }
    public string? Lastname { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Gender { get; set; }

    [JsonConverter(typeof(LegacyDateTimeConverter))]
    public DateTime? Dob { get; set; }
    public string? Tel { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public decimal? Salary { get; set; }

    /// <summary>
    /// Create only: initial login password. When omitted a temporary password is generated.
    /// Ignored on update (use POST /api/employee/setPassword).
    /// </summary>
    public string? Password { get; set; }
}

/// <summary>Express shape: <c>{ list: [...], role: [...] }</c>.</summary>
public class EmployeeListResponse
{
    [JsonPropertyName("list")]
    public IReadOnlyList<EmployeeDto> List { get; set; } = Array.Empty<EmployeeDto>();

    [JsonPropertyName("role")]
    public IReadOnlyList<RoleDto> Role { get; set; } = Array.Empty<RoleDto>();
}

/// <summary>
/// <c>{ message, data, temporary_password? }</c>. temporary_password is only present on create, only when the
/// caller did not supply a Password, and is never returned by any other endpoint.
/// </summary>
public class EmployeeCreateResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public EmployeeDto? Data { get; set; }

    [JsonPropertyName("temporary_password")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? TemporaryPassword { get; set; }
}
