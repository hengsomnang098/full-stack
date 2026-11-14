using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.PaymentMethods;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class OrderPaymentMethodEndpoints
{
    public static IEndpointRouteBuilder MapOrderPaymentMethodEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/order_payment_method").WithTags("OrderPaymentMethod").RequireAuthorization();

        group.MapGet("/list", async (string? txt_search, string? status, IOrderPaymentMethodService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, ct)));

        group.MapGet("/getone", async (int? id, IOrderPaymentMethodService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("/create", async (OrderPaymentMethodRequest req, IOrderPaymentMethodService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (OrderPaymentMethodRequest req, IOrderPaymentMethodService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, IOrderPaymentMethodService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
