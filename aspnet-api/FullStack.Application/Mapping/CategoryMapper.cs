using FullStack.Application.DTOs.Categories;
using FullStack.Domain.Entities;

namespace FullStack.Application.Mapping;

public static class CategoryMapper
{
    public static CategoryDto ToDto(Category e) => new()
    {
        Id = e.Id,
        Name = e.Name,
        Description = e.Description,
        Status = e.Status,
        CreateAt = e.CreateAt
    };

    public static Category ToEntity(CategoryRequest r) => new()
    {
        Name = r.Name!.Trim(),
        Description = r.Description,
        Status = r.Status ?? true
    };

    public static void Apply(Category e, CategoryRequest r)
    {
        e.Name = r.Name!.Trim();
        e.Description = r.Description;
        e.Status = r.Status ?? e.Status;
    }
}
