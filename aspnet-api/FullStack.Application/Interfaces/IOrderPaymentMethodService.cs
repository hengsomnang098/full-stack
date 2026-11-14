using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.PaymentMethods;

namespace FullStack.Application.Interfaces;

public interface IOrderPaymentMethodService
{
    Task<ServiceResult<ListResponse<OrderPaymentMethodDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<OrderPaymentMethodDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> CreateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> UpdateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
