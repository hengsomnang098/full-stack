using FullStack.Application.Common;
using FullStack.Application.DTOs.Categories;
using FullStack.Application.DTOs.Common;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class CategoryService : ICategoryService
{
    private readonly IAppDbContext _db;
    private readonly ILogger<CategoryService> _logger;

    public CategoryService(IAppDbContext db, ILogger<CategoryService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<ServiceResult<CategoryListResponse>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default)
    {
        var query = _db.Categories.AsNoTracking().AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(c => c.Name.Contains(search) || (c.Description != null && c.Description.Contains(search)));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(c => c.Status == statusFilter.Value);

        var items = await query.OrderByDescending(c => c.Id).ToListAsync(ct);
        var total = await _db.Categories.CountAsync(ct);

        return ServiceResult<CategoryListResponse>.Ok(new CategoryListResponse
        {
            Count = new[] { new CountItem { Total = total } },
            List = items.Select(CategoryMapper.ToDto).ToList()
        });
    }

    public async Task<ServiceResult<DataResponse<CategoryDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<CategoryDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<CategoryDto>>.Ok(DataResponse<CategoryDto>.From(CategoryMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<CategoryDto>>> CreateAsync(CategoryRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<CategoryDto>>.FailValidation(errors);

        var entity = CategoryMapper.ToEntity(request);
        _db.Categories.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Category create failed");
            return ServiceResult<MessageDataResponse<CategoryDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<CategoryDto>>.Ok(
            MessageDataResponse<CategoryDto>.From("Insert success", CategoryMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<CategoryDto>>> UpdateAsync(CategoryRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<CategoryDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<CategoryDto>>.FailValidation(errors);

        var entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<CategoryDto>>.Fail("Not found", 404);

        CategoryMapper.Apply(entity, request);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Category update failed");
            return ServiceResult<MessageDataResponse<CategoryDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<CategoryDto>>.Ok(
            MessageDataResponse<CategoryDto>.From("Update success", CategoryMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        _db.Categories.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Category delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private static Dictionary<string, string> Validate(CategoryRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Name)) errors["Name"] = "Name required!";
        return errors;
    }
}
