using FullStack.Application.Common;
using FullStack.Application.Interfaces;

namespace FullStack.Api.Endpoints;

public static class InvoiceEndpoints
{
    public static IEndpointRouteBuilder MapInvoiceEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api").WithTags("Invoice").RequireAuthorization();

        group.MapGet("/invoice", async (IInvoiceService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(ct)));

        // Id is bound as string because the React client may send "invoice_details/ 5" (note the space).
        group.MapGet("/invoice_details/{Id}", async (string Id, IInvoiceService svc, CancellationToken ct) =>
            LegacyParse.ToInt(Id) is { } invoiceId
                ? ApiResults.From(await svc.GetDetailsAsync(invoiceId, ct))
                : ApiResults.Error("Invalid invoice id."));

        return app;
    }
}
