using System.Text.Json.Serialization;

namespace FullStack.Application.DTOs.Common;

public class ListResponse<T>
{
    [JsonPropertyName("list")]
    public IReadOnlyList<T> List { get; set; } = Array.Empty<T>();

    [JsonPropertyName("total")]
    public int Total { get; set; }

    public static ListResponse<T> From(IReadOnlyList<T> items) => new() { List = items, Total = items.Count };
}
