using FullStack.Application.DTOs.Pos;
using FullStack.Application.Interfaces;

namespace FullStack.Api.Endpoints;

public static class PosEndpoints
{
    public static IEndpointRouteBuilder MapPosEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/pos").WithTags("POS").RequireAuthorization();

        group.MapGet("/initInfo", async (IPosService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetInitInfoAsync(ct)));

        group.MapGet("/searchProduct", async (string? txt_search, IPosService svc, CancellationToken ct) =>
            ApiResults.From(await svc.SearchProductAsync(txt_search, ct)));

        group.MapPost("/checkout", async (PosCheckoutRequest req, IPosService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CheckoutAsync(req, ct)));

        return app;
    }
}
