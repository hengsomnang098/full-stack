using System.Text.Json.Serialization;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Application.DTOs.Customers;

public class CustomerDto
{
    public int Id { get; set; }
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
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}

public class CustomerRequest
{
    public int? Id { get; set; }
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
}
