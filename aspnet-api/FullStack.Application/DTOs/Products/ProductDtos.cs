using System.Text.Json.Serialization;
using FullStack.Application.DTOs.Categories;
using FullStack.Application.DTOs.Common;

namespace FullStack.Application.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }
    public int? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? Qty { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }
    public string? Image { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}

/// <summary>
/// Used for both JSON bodies and multipart/form-data (the endpoint parses the form leniently into this type;
/// the image file itself is passed separately).
/// </summary>
public class ProductRequest
{
    public int? Id { get; set; }
    public int? CategoryId { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Qty { get; set; }
    public decimal? Price { get; set; }
    public decimal? Discount { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }

    /// <summary>Existing image file name sent back by the client (kept when no new file is uploaded).</summary>
    public string? PreImage { get; set; }
}

/// <summary>Express shape: <c>{ list: [...], category: [...] }</c>.</summary>
public class ProductListResponse
{
    [JsonPropertyName("list")]
    public IReadOnlyList<ProductDto> List { get; set; } = Array.Empty<ProductDto>();

    [JsonPropertyName("category")]
    public IReadOnlyList<CategoryDto> Category { get; set; } = Array.Empty<CategoryDto>();
}
