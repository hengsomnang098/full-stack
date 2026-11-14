using FullStack.Api.Application.DTOs.Roles;
using FullStack.Api.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class RoleEndpoints
{
    public static IEndpointRouteBuilder MapRoleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/role").WithTags("Role").RequireAuthorization();

        group.MapGet("/getList", async (string? txt_search, string? status, IRoleService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, ct)));

        group.MapPost("/create", async (RoleRequest req, IRoleService svc, CancellationToken ct) =>
            ApiResults.From(await svc.CreateAsync(req, ct)));

        group.MapPut("/update", async (RoleRequest req, IRoleService svc, CancellationToken ct) =>
            ApiResults.From(await svc.UpdateAsync(req, ct)));

        group.MapDelete("/delete", async ([FromBody] RoleIdRequest? body, string? id, IRoleService svc, CancellationToken ct) =>
        {
            var roleId = body?.Id ?? id;
            return string.IsNullOrWhiteSpace(roleId)
                ? ApiResults.MissingId()
                : ApiResults.From(await svc.DeleteAsync(roleId, ct));
        });

        return app;
    }
}
