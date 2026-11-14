using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Suppliers;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class SupplierEndpoints
{
    public static IEndpointRouteBuilder MapSupplierEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/supplier").WithTags("Supplier").RequireAuthorization();

        group.MapGet("/getList", async (ISupplierService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(ct)));

        group.MapGet("/getById", async (int? id, ISupplierService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("/create", async (SupplierRequest req, ISupplierService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (SupplierRequest req, ISupplierService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, ISupplierService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
