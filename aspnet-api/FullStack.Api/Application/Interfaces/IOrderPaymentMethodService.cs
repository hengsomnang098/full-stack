using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.PaymentMethods;

namespace FullStack.Api.Application.Interfaces;

public interface IOrderPaymentMethodService
{
    Task<ServiceResult<ListResponse<OrderPaymentMethodDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<OrderPaymentMethodDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> CreateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<OrderPaymentMethodDto>>> UpdateAsync(OrderPaymentMethodRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
