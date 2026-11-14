using System.Text.Json.Serialization;

namespace FullStack.Api.Application.DTOs.Common;

public class ApiMessageResponse
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    public ApiMessageResponse() { }
    public ApiMessageResponse(string message) => Message = message;
}
