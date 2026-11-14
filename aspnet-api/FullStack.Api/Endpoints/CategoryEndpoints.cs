using FullStack.Api.Application.DTOs.Categories;
using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class CategoryEndpoints
{
    public static IEndpointRouteBuilder MapCategoryEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/category").WithTags("Category").RequireAuthorization();

        group.MapGet("", async (string? txt_search, string? status, ICategoryService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, ct)));

        group.MapGet("/getById", async (int? id, ICategoryService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("", async (CategoryRequest req, ICategoryService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("", async (CategoryRequest req, ICategoryService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("", async ([FromBody] IdRequest? body, int? id, ICategoryService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
