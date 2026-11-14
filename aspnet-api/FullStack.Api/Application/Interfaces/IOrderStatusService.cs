using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.OrderStatuses;

namespace FullStack.Api.Application.Interfaces;

public interface IOrderStatusService
{
    Task<ServiceResult<ListResponse<OrderStatusDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<OrderStatusDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> CreateAsync(OrderStatusRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> UpdateAsync(OrderStatusRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
