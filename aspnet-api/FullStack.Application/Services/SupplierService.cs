using FullStack.Application.Common;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Suppliers;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class SupplierService : ISupplierService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<SupplierService> _logger;

    public SupplierService(IAppDbContext db, ICurrentUserService currentUser, ILogger<SupplierService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<ListResponse<SupplierDto>>> GetListAsync(CancellationToken ct = default)
    {
        var items = await _db.Suppliers.AsNoTracking().OrderBy(s => s.Id).ToListAsync(ct);
        return ServiceResult<ListResponse<SupplierDto>>.Ok(
            ListResponse<SupplierDto>.From(items.Select(SupplierMapper.ToDto).ToList()));
    }

    public async Task<ServiceResult<DataResponse<SupplierDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Suppliers.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<SupplierDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<SupplierDto>>.Ok(DataResponse<SupplierDto>.From(SupplierMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<SupplierDto>>> CreateAsync(SupplierRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<SupplierDto>>.FailValidation(errors);

        var entity = SupplierMapper.ToEntity(request, _currentUser.EmployeeId);
        _db.Suppliers.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Supplier create failed");
            return ServiceResult<MessageDataResponse<SupplierDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<SupplierDto>>.Ok(
            MessageDataResponse<SupplierDto>.From("Insert success", SupplierMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<SupplierDto>>> UpdateAsync(SupplierRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<SupplierDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<SupplierDto>>.FailValidation(errors);

        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<SupplierDto>>.Fail("Not found", 404);

        SupplierMapper.Apply(entity, request, _currentUser.EmployeeId);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Supplier update failed");
            return ServiceResult<MessageDataResponse<SupplierDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<SupplierDto>>.Ok(
            MessageDataResponse<SupplierDto>.From("Update success", SupplierMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Suppliers.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        _db.Suppliers.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Supplier delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private static Dictionary<string, string> Validate(SupplierRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Name)) errors["Name"] = "Name required!";
        else if (string.IsNullOrWhiteSpace(r.Tel)) errors["Tel"] = "Tel required!";
        else if (string.IsNullOrWhiteSpace(r.Email)) errors["Email"] = "Email required!";
        return errors;
    }
}
