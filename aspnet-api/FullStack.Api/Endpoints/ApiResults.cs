using System.Text.Json;
using FullStack.Api.Application.DTOs.Common;

namespace FullStack.Api.Endpoints;

/// <summary>
/// Writes business responses using PascalCase property names (Id, Name, RoleName ...) like the Express API
/// returned MySQL column names. Envelope keys (list, data, message, error ...) are lower-case via JsonPropertyName.
/// </summary>
public static class ApiResults
{
    public static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        PropertyNamingPolicy = null,
        DictionaryKeyPolicy = null
    };

    public static IResult From<T>(ServiceResult<T> result) =>
        result.Success
            ? Results.Json(result.Data, JsonOptions)
            : Error(result.ErrorBody ?? result.Error ?? "Error", result.StatusCode);

    public static IResult Error(object message, int statusCode = 400) =>
        Results.Json(new ApiErrorResponse(message), JsonOptions, statusCode: statusCode);

    public static IResult MissingId() => Error("Id is required.", 400);

    /// <summary>Id from JSON body (Express style) or ?Id= query string.</summary>
    public static int? ResolveId(IdRequest? body, int? query) => body?.Id ?? query;
}
