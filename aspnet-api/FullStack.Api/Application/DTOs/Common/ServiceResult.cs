namespace FullStack.Api.Application.DTOs.Common;

public class ServiceResult<T>
{
    public bool Success { get; private init; }
    public T? Data { get; private init; }
    public string? Error { get; private init; }

    /// <summary>Optional structured error body (e.g. field => message map for validation errors).</summary>
    public object? ErrorBody { get; private init; }
    public int StatusCode { get; private init; }

    public static ServiceResult<T> Ok(T data) => new() { Success = true, Data = data, StatusCode = 200 };
    public static ServiceResult<T> Fail(string error, int statusCode = 400) =>
        new() { Success = false, Error = error, StatusCode = statusCode };

    public static ServiceResult<T> FailValidation(IReadOnlyDictionary<string, string> errors, int statusCode = 400) =>
        new() { Success = false, Error = "Validation failed.", ErrorBody = errors, StatusCode = statusCode };
}
