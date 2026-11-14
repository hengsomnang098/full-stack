using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Employees;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

/// <summary>Employee business CRUD. Auth endpoints (login/refresh/setPassword/me/logout) are in <see cref="AuthEndpoints"/>.</summary>
public static class EmployeeEndpoints
{
    public static IEndpointRouteBuilder MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employee").WithTags("Employee").RequireAuthorization();

        group.MapGet("/getlist", async (string? txt_search, string? status, string? role_id,
                IEmployeeService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, role_id, ct)));

        group.MapGet("/getone", async (int? id, IEmployeeService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        group.MapPost("/create", async (EmployeeRequest req, IEmployeeService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (EmployeeRequest req, IEmployeeService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, IEmployeeService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }
}
