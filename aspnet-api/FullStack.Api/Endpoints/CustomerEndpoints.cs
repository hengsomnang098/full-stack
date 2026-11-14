using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Customers;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class CustomerEndpoints
{
    public static IEndpointRouteBuilder MapCustomerEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/customer").WithTags("Customer").RequireAuthorization();

        group.MapGet("/getlist", async (string? txt_search, string? status, ICustomerService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, ct)));

        group.MapGet("/getById", async (int? id, ICustomerService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("/create", async (CustomerRequest req, ICustomerService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (CustomerRequest req, ICustomerService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, ICustomerService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
