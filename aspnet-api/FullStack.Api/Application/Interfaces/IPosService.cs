using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Pos;

namespace FullStack.Api.Application.Interfaces;

public interface IPosService
{
    Task<ServiceResult<PosInitResponse>> GetInitInfoAsync(CancellationToken ct = default);
    Task<ServiceResult<PosSearchResponse>> SearchProductAsync(string? txtSearch, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<PosCheckoutResult>>> CheckoutAsync(PosCheckoutRequest request, CancellationToken ct = default);
}
