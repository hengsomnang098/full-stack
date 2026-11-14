using FullStack.Application.Common;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.OrderStatuses;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class OrderStatusService : IOrderStatusService
{
    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<OrderStatusService> _logger;

    public OrderStatusService(IAppDbContext db, ICurrentUserService currentUser, ILogger<OrderStatusService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<ListResponse<OrderStatusDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default)
    {
        var query = _db.OrderStatuses.AsNoTracking().AsQueryable();

        var search = LegacyParse.Text(txtSearch);
        if (search is not null)
            query = query.Where(s => s.Name.Contains(search) || s.Code.Contains(search));

        var statusFilter = LegacyParse.StatusFilter(status);
        if (statusFilter.HasValue)
            query = query.Where(s => s.Status == statusFilter.Value);

        var items = await query.OrderBy(s => s.Id).ToListAsync(ct);
        return ServiceResult<ListResponse<OrderStatusDto>>.Ok(
            ListResponse<OrderStatusDto>.From(items.Select(OrderStatusMapper.ToDto).ToList()));
    }

    public async Task<ServiceResult<DataResponse<OrderStatusDto>>> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.OrderStatuses.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id, ct);
        return entity is null
            ? ServiceResult<DataResponse<OrderStatusDto>>.Fail("Not found", 404)
            : ServiceResult<DataResponse<OrderStatusDto>>.Ok(DataResponse<OrderStatusDto>.From(OrderStatusMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> CreateAsync(OrderStatusRequest request, CancellationToken ct = default)
    {
        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<OrderStatusDto>>.FailValidation(errors);

        var entity = OrderStatusMapper.ToEntity(request, _currentUser.EmployeeId);
        _db.OrderStatuses.Add(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderStatus create failed");
            return ServiceResult<MessageDataResponse<OrderStatusDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<OrderStatusDto>>.Ok(
            MessageDataResponse<OrderStatusDto>.From("Insert success", OrderStatusMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> UpdateAsync(OrderStatusRequest request, CancellationToken ct = default)
    {
        if (request.Id is null) return ServiceResult<MessageDataResponse<OrderStatusDto>>.Fail("Id is required.");

        var errors = Validate(request);
        if (errors.Count > 0) return ServiceResult<MessageDataResponse<OrderStatusDto>>.FailValidation(errors);

        var entity = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.Id == request.Id, ct);
        if (entity is null) return ServiceResult<MessageDataResponse<OrderStatusDto>>.Fail("Not found", 404);

        OrderStatusMapper.Apply(entity, request, _currentUser.EmployeeId);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderStatus update failed");
            return ServiceResult<MessageDataResponse<OrderStatusDto>>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<MessageDataResponse<OrderStatusDto>>.Ok(
            MessageDataResponse<OrderStatusDto>.From("Update success", OrderStatusMapper.ToDto(entity)));
    }

    public async Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.OrderStatuses.FirstOrDefaultAsync(s => s.Id == id, ct);
        if (entity is null) return ServiceResult<ApiMessageResponse>.Fail("Not found", 404);

        _db.OrderStatuses.Remove(entity);

        try
        {
            await _db.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "OrderStatus delete failed");
            return ServiceResult<ApiMessageResponse>.Fail(mapped.Message, mapped.StatusCode);
        }

        return ServiceResult<ApiMessageResponse>.Ok(new ApiMessageResponse("Remove success"));
    }

    private static Dictionary<string, string> Validate(OrderStatusRequest r)
    {
        var errors = new Dictionary<string, string>();
        if (string.IsNullOrWhiteSpace(r.Name)) errors["Name"] = "Name required!";
        if (string.IsNullOrWhiteSpace(r.Code)) errors["Code"] = "Code required!";
        return errors;
    }
}
