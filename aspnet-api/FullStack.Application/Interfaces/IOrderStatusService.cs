using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.OrderStatuses;

namespace FullStack.Application.Interfaces;

public interface IOrderStatusService
{
    Task<ServiceResult<ListResponse<OrderStatusDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<OrderStatusDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> CreateAsync(OrderStatusRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderStatusDto>>> UpdateAsync(OrderStatusRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
