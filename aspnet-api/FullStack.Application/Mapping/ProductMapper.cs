using FullStack.Application.Common;
using FullStack.Application.DTOs.Products;
using FullStack.Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace FullStack.Application.Mapping;

public static class ProductMapper
{
    public static ProductDto ToDto(Product e) => new()
    {
        Id = e.Id,
        CategoryId = e.CategoryId,
        CategoryName = e.Category?.Name,
        Name = e.Name,
        Description = e.Description,
        Qty = e.Qty,
        Price = e.Price,
        Discount = e.Discount,
        Image = e.Image,
        Status = e.Status,
        CreateAt = e.CreateAt,
        CreateBy = e.CreateBy,
        UpdateAt = e.UpdateAt,
        UpdateBy = e.UpdateBy
    };

    public static Product ToEntity(ProductRequest r, string? image, int? employeeId) => new()
    {
        CategoryId = r.CategoryId,
        Name = r.Name!.Trim(),
        Description = LegacyParse.Text(r.Description),
        Qty = r.Qty,
        Price = r.Price,
        Discount = r.Discount ?? 0,
        Image = image,
        Status = r.Status ?? true,
        CreateBy = employeeId
    };

    /// <summary>Applies the request to the entity. Image is handled by the service.</summary>
    public static void Apply(Product e, ProductRequest r, int? employeeId)
    {
        e.CategoryId = r.CategoryId;
        e.Name = r.Name!.Trim();
        e.Description = LegacyParse.Text(r.Description);
        e.Qty = r.Qty;
        e.Price = r.Price;
        e.Discount = r.Discount ?? 0;
        e.Status = r.Status ?? e.Status;
        e.UpdateAt = DateTime.UtcNow;
        e.UpdateBy = employeeId;
    }

    /// <summary>Lenient parse of the multipart form sent by the React client (all values arrive as strings).</summary>
    public static ProductRequest FromForm(IFormCollection form) => new()
    {
        Id = LegacyParse.ToInt(form["Id"]),
        CategoryId = LegacyParse.ToInt(form["CategoryId"]),
        Name = LegacyParse.Text(form["Name"]),
        Description = LegacyParse.Text(form["Description"]),
        Qty = LegacyParse.ToInt(form["Qty"]),
        Price = LegacyParse.ToDecimal(form["Price"]),
        Discount = LegacyParse.ToDecimal(form["Discount"]),
        Status = LegacyParse.ToBool(form["Status"]),
        PreImage = LegacyParse.Text(form["PreImage"])
    };
}
