using FullStack.Application.Common;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Products;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class ProductService : IProductService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly IFileStorageService _files;
    private readonly ILogger<ProductService> _logger;

    public ProductService(IAppDbContext db, ICurrentUserService currentUser, IFileStorageService files, ILogger<ProductService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _files = files;
        _logger = logger;
    }

    public async Task<ServiceResult<ProductListResponse>> GetListAsync(string? txtSearch, string? status, string? categoryId, CancellationToken ct = default)
    {
        var query = _db.Products.AsNoTracking().Include(p => p.Category).AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(p => p.Name.Contains(search));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(p => p.Status == statusFilter.Value);

        var categoryFilter = LegacyParse.IdFilter(categoryId);
        if (categoryFilter.HasValue)
            query = query.Where(p => p.CategoryId == categoryFilter.Value);

        var items = await query.OrderBy(p => p.Id).ToListAsync(ct);
        var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);

        return ServiceResult<ProductListResponse>.Ok(new ProductListResponse
        {
            List = items.Select(ProductMapper.ToDto).ToList(),
            Category = categories.Select(CategoryMapper.ToDto).ToList()
        });
    }

    public async Task<ServiceResult<DataResponse<ProductDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Products.AsNoTracking().Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<ProductDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<ProductDto>>.Ok(DataResponse<ProductDto>.From(ProductMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<ProductDto>>> CreateAsync(ProductRequest request, IFormFile? image, CancellationToken ct = default)
    {
        var errors = Validate(request, requireId: false);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<ProductDto>>.FailValidation(errors);

        string? fileName = null;
        if (image is { Length: > 0 })
        {
            try { fileName = await _files.SaveImageAsync(image, ct); }
            catch (InvalidUploadException ex) { return ServiceResult<MessageDataResponse<ProductDto>>.Fail(ex.Message); }
        }

        var entity = ProductMapper.ToEntity(request, fileName, _currentUser.EmployeeId);
        _db.Products.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _files.Delete(fileName);
            _logger.LogWarning(ex, "Product create failed");
            return ServiceResult<MessageDataResponse<ProductDto>>.Fail(mapped.Message, mapped.StatusCode);
        }
        catch
        {
            _files.Delete(fileName);
            throw;
        }

        await _db.Entry(entity).Reference(p => p.Category).LoadAsync(ct);
        return ServiceResult<MessageDataResponse<ProductDto>>.Ok(
            MessageDataResponse<ProductDto>.From("Create successfully", ProductMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<ProductDto>>> UpdateAsync(ProductRequest request, IFormFile? image, CancellationToken ct = default)
    {
        var errors = Validate(request, requireId: true);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<ProductDto>>.FailValidation(errors);

        var entity = await _db.Products.FirstOrDefaultAsync(p => p.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<ProductDto>>.Fail("Not found", 404);

        string? newFile = null;
        if (image is { Length: > 0 })
        {
            try { newFile = await _files.SaveImageAsync(image, ct); }
            catch (InvalidUploadException ex) { return ServiceResult<MessageDataResponse<ProductDto>>.Fail(ex.Message); }
        }

        var oldFile = entity.Image;
        ProductMapper.Apply(entity, request, _currentUser.EmployeeId);
        if (newFile is not null) entity.Image = newFile;

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _files.Delete(newFile);
            _logger.LogWarning(ex, "Product update failed");
            return ServiceResult<MessageDataResponse<ProductDto>>.Fail(mapped.Message, mapped.StatusCode);
        }
        catch
        {
            _files.Delete(newFile);
            throw;
        }

        if (newFile is not null) _files.Delete(oldFile);

        await _db.Entry(entity).Reference(p => p.Category).LoadAsync(ct);
        return ServiceResult<MessageDataResponse<ProductDto>>.Ok(
            MessageDataResponse<ProductDto>.From("Update successfully", ProductMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        var file = entity.Image;
        _db.Products.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Product delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        _files.Delete(file);
        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Delete successfully"));
    }

    private static Dictionary<string, string> Validate(ProductRequest r, bool requireId)
    {
        var errors = new Dictionary<string, string>();
        if (requireId && r.Id is null) errors["Id"] = "Id is require.";
        if (string.IsNullOrWhiteSpace(r.Name)) errors["Name"] = "Name is require.";
        if (r.Qty is null) errors["Qty"] = "Qty is require.";
        if (r.Price is null) errors["Price"] = "Price is require.";
        return errors;
    }
}
