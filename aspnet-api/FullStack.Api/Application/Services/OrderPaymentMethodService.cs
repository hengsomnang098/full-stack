using FullStack.Api.Application.Common;
using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.PaymentMethods;
using FullStack.Api.Application.Interfaces;
using FullStack.Api.Application.Mapping;
using FullStack.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Application.Services;

public class OrderPaymentMethodService : IOrderPaymentMethodService
{
    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<OrderPaymentMethodService> _logger;

    public OrderPaymentMethodService(AppDbContext db, ICurrentUserService currentUser, ILogger<OrderPaymentMethodService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<ListResponse<OrderPaymentMethodDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default)
    {
        var query = _db.OrderPaymentMethods.AsNoTracking().AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(s => s.Name.Contains(search) || s.Code.Contains(search));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(s => s.Status == statusFilter.Value);

        var items = await query.OrderBy(s => s.Id).ToListAsync(ct);
        return ServiceResult<ListResponse<OrderPaymentMethodDto>>.Ok(
            ListResponse<OrderPaymentMethodDto>.From(items.Select(OrderPaymentMethodMapper.ToDto).ToList()));
    }

    public async Task<ServiceResult<DataResponse<OrderPaymentMethodDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.OrderPaymentMethods.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<OrderPaymentMethodDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<OrderPaymentMethodDto>>.Ok(
                DataResponse<OrderPaymentMethodDto>.From(OrderPaymentMethodMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> CreateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.FailValidation(errors);

        var entity = OrderPaymentMethodMapper.ToEntity(request, _currentUser.EmployeeId);
        _db.OrderPaymentMethods.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderPaymentMethod create failed");
            return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Ok(
            MessageDataResponse<OrderPaymentMethodDto>.From("Insert success", OrderPaymentMethodMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> UpdateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.FailValidation(errors);

        var entity = await _db.OrderPaymentMethods.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Fail("Not found", 404);

        OrderPaymentMethodMapper.Apply(entity, request, _currentUser.EmployeeId);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderPaymentMethod update failed");
            return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>.Ok(
            MessageDataResponse<OrderPaymentMethodDto>.From("Update success", OrderPaymentMethodMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.OrderPaymentMethods.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        _db.OrderPaymentMethods.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderPaymentMethod delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private static Dictionary<string, string> Validate(OrderPaymentMethodRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Name)) errors["Name"] = "Name required!";
        if (string.IsNullOrWhiteSpace(r.Code)) errors["Code"] = "Code required!";
        return errors;
    }
}
