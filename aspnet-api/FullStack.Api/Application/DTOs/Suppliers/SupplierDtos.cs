using System.Text.Json.Serialization;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Application.DTOs.Suppliers;

public class SupplierDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Tel { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string? WebsiteUrl { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}

public class SupplierRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Tel { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public string? WebsiteUrl { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
}
