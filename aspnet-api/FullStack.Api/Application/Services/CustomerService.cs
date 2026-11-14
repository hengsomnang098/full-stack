using FullStack.Api.Application.Common;
using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Customers;
using FullStack.Api.Application.Interfaces;
using FullStack.Api.Application.Mapping;
using FullStack.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<CustomerService> _logger;

    public CustomerService(AppDbContext db, ICurrentUserService currentUser, ILogger<CustomerService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<ListResponse<CustomerDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default)
    {
        var query = _db.Customers.AsNoTracking().AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(c => c.Firstname.Contains(search) || c.Lastname.Contains(search));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(c => c.Status == statusFilter.Value);

        var items = await query.OrderByDescending(c => c.Id).ToListAsync(ct);
        return ServiceResult<ListResponse<CustomerDto>>.Ok(
            ListResponse<CustomerDto>.From(items.Select(CustomerMapper.ToDto).ToList()));
    }

    public async Task<ServiceResult<DataResponse<CustomerDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Customers.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<CustomerDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<CustomerDto>>.Ok(DataResponse<CustomerDto>.From(CustomerMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<CustomerDto>>> CreateAsync(CustomerRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<CustomerDto>>.FailValidation(errors);

        var entity = CustomerMapper.ToEntity(request, _currentUser.EmployeeId);
        _db.Customers.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Customer create failed");
            return ServiceResult<MessageDataResponse<CustomerDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<CustomerDto>>.Ok(
            MessageDataResponse<CustomerDto>.From("Create success", CustomerMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<CustomerDto>>> UpdateAsync(CustomerRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<CustomerDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<CustomerDto>>.FailValidation(errors);

        var entity = await _db.Customers.FirstOrDefaultAsync(c => c.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<CustomerDto>>.Fail("Not found", 404);

        CustomerMapper.Apply(entity, request, _currentUser.EmployeeId);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Customer update failed");
            return ServiceResult<MessageDataResponse<CustomerDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<CustomerDto>>.Ok(
            MessageDataResponse<CustomerDto>.From("Update success", CustomerMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        _db.Customers.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "Customer delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private static Dictionary<string, string> Validate(CustomerRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Firstname)) errors["Firstname"] = "Firstname required!";
        if (string.IsNullOrWhiteSpace(r.Lastname)) errors["Lastname"] = "Lastname required!";
        if (string.IsNullOrWhiteSpace(r.Tel)) errors["Tel"] = "Tel required!";
        return errors;
    }
}
