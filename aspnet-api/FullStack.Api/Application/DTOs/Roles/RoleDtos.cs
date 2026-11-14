using System.Text.Json.Serialization;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Application.DTOs.Roles;

/// <summary>Maps ASP.NET IdentityRole for the existing Role UI.</summary>
public class RoleDto
{
    /// <summary>Identity role Id (string).</summary>
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;

    /// <summary>Kept for UI compatibility; same as Name for Identity roles.</summary>
    public string Code { get; set; } = string.Empty;

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; } = true;
}

public class RoleRequest
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
}

public class RoleIdRequest
{
    public string? Id { get; set; }
}
