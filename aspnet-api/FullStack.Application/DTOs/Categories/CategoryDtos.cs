using System.Text.Json.Serialization;
using FullStack.Application.DTOs.Common;

namespace FullStack.Application.DTOs.Categories;

public class CategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public DateTime CreateAt { get; set; }
}

public class CategoryRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
}

/// <summary>Express shape: <c>{ count: [{ total }], list: [...] }</c>.</summary>
public class CategoryListResponse
{
    [JsonPropertyName("count")]
    public IReadOnlyList<CountItem> Count { get; set; } = Array.Empty<CountItem>();

    [JsonPropertyName("list")]
    public IReadOnlyList<CategoryDto> List { get; set; } = Array.Empty<CategoryDto>();
}
