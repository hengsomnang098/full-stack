using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Pos;

namespace FullStack.Application.Interfaces;

public interface IPosService
{
    Task<ServiceResult<PosInitResponse>> GetInitInfoAsync(CancellationToken ct = default);
    Task<ServiceResult<PosSearchResponse>> SearchProductAsync(string? txtSearch, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<PosCheckoutResult>>> CheckoutAsync(PosCheckoutRequest request, CancellationToken ct = default);
}
