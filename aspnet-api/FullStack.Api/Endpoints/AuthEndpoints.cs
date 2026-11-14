using FullStack.Api.Application.DTOs.Auth;
using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.Interfaces;

namespace FullStack.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employee").WithTags("Auth");

        group.MapPost("/login", async (LoginRequest req, IAuthService auth, CancellationToken ct) =>
            ToResult(await auth.LoginAsync(req, ct)))
            .AllowAnonymous();

        group.MapPost("/refreshtoken", async (IAuthService auth, CancellationToken ct) =>
            ToResult(await auth.RefreshTokenAsync(ct)))
            .AllowAnonymous();

        group.MapPost("/setPassword", async (SetPasswordRequest req, IAuthService auth, CancellationToken ct) =>
            ToResult(await auth.SetPasswordAsync(req, ct)))
            .RequireAuthorization();

        group.MapPost("/logout", (IAuthService auth) => Results.Ok(auth.Logout()))
            .RequireAuthorization();

        group.MapGet("/me", async (IAuthService auth, CancellationToken ct) =>
            ToResult(await auth.GetCurrentUserAsync(ct)))
            .RequireAuthorization();

        // Legacy-compatible alias
        app.MapPost("/api/refresh_token", async (IAuthService auth, CancellationToken ct) =>
            ToResult(await auth.RefreshTokenAsync(ct)))
            .WithTags("Auth")
            .AllowAnonymous();

        return app;
    }

    private static IResult ToResult<T>(ServiceResult<T> result) =>
        result.Success
            ? Results.Ok(result.Data)
            : Results.Json(
                new { error = true, message = result.Error ?? "Error" },
                statusCode: result.StatusCode);
}
