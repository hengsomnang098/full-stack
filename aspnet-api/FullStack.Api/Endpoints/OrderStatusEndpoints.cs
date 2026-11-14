using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.OrderStatuses;
using FullStack.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class OrderStatusEndpoints
{
    public static IEndpointRouteBuilder MapOrderStatusEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/order_status").WithTags("OrderStatus").RequireAuthorization();

        group.MapGet("/list", async (string? txt_search, string? status, IOrderStatusService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, ct)));

        group.MapGet("/getone", async (int? id, IOrderStatusService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("/create", async (OrderStatusRequest req, IOrderStatusService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (OrderStatusRequest req, IOrderStatusService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, IOrderStatusService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
