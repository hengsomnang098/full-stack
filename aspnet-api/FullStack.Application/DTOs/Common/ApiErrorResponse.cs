using System.Text.Json.Serialization;

namespace FullStack.Application.DTOs.Common;

/// <summary>Express-style error body: <c>{ "error": true, "message": ... }</c> (message is a string or a field map).</summary>
public class ApiErrorResponse
{
    [JsonPropertyName("error")]
    public bool Error { get; set; } = true;

    [JsonPropertyName("message")]
    public object? Message { get; set; }

    public ApiErrorResponse() { }
    public ApiErrorResponse(object? message) => Message = message;
}

/// <summary>Express-style body: <c>{ "message": "...", "data": ... }</c>.</summary>
public class MessageDataResponse<T>
{
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public static MessageDataResponse<T> From(string message, T? data) => new() { Message = message, Data = data };
}

/// <summary>Express-style body: <c>{ "data": ... }</c>.</summary>
public class DataResponse<T>
{
    [JsonPropertyName("data")]
    public T? Data { get; set; }

    public static DataResponse<T> From(T? data) => new() { Data = data };
}

/// <summary>Body for POST/PUT/DELETE endpoints that identify a row by Id (JSON body or ?Id= query).</summary>
public class IdRequest
{
    public int? Id { get; set; }
}

/// <summary>Row used by Express' <c>count</c> property: <c>[{ "total": 12 }]</c>.</summary>
public class CountItem
{
    [JsonPropertyName("total")]
    public int Total { get; set; }
}
