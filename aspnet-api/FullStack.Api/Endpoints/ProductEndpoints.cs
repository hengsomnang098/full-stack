using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Products;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using Microsoft.AspNetCore.Mvc;

namespace FullStack.Api.Endpoints;

public static class ProductEndpoints
{
    public static IEndpointRouteBuilder MapProductEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/product").WithTags("Product").RequireAuthorization();

        group.MapGet("/getList", async (string? txt_search, string? status, string? category_id,
                IProductService svc, CancellationToken ct) =>
            ApiResults.From(await svc.GetListAsync(txt_search, status, category_id, ct)));

        group.MapGet("/getById", async (int? id, IProductService svc, CancellationToken ct) =>
            id is null ? ApiResults.MissingId() : ApiResults.From(await svc.GetByIdAsync(id.Value, ct)));

        // multipart/form-data (field "image" = optional file) or application/json.
        group.MapPost("/create", async Task<IResult> (HttpRequest http, IProductService svc, CancellationToken ct) =>
        {
            var (request, image, error) = await ReadRequestAsync(http, ct);
            return error ?? ApiResults.From(await svc.CreateAsync(request!, image, ct));
        });

        group.MapPut("/update", async Task<IResult> (HttpRequest http, IProductService svc, CancellationToken ct) =>
        {
            var (request, image, error) = await ReadRequestAsync(http, ct);
            return error ?? ApiResults.From(await svc.UpdateAsync(request!, image, ct));
        });

        group.MapDelete("/delete", async ([FromBody] IdRequest? body, int? id, IProductService svc, CancellationToken ct) =>
            ApiResults.ResolveId(body, id) is { } resolved
                ? ApiResults.From(await svc.DeleteAsync(resolved, ct))
                : ApiResults.MissingId());

        return app;
    }

    private static async Task<(ProductRequest? Request, IFormFile? Image, IResult? Error)> ReadRequestAsync(
        HttpRequest http, CancellationToken ct)
    {
        if (http.HasFormContentType)
        {
            var form = await http.ReadFormAsync(ct);
            return (ProductMapper.FromForm(form), form.Files.GetFile("image"), null);
        }

        if (http.HasJsonContentType())
        {
            try
            {
                var body = await http.ReadFromJsonAsync<ProductRequest>(ct);
                return body is null
                    ? (null, null, ApiResults.Error("Request body is required."))
                    : (body, null, null);
            }
            catch (System.Text.Json.JsonException)
            {
                return (null, null, ApiResults.Error("Invalid JSON body."));
            }
        }

        return (null, null, ApiResults.Error("Content-Type must be multipart/form-data or application/json.", 415));
    }
}
