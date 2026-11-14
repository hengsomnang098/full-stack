using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Suppliers;

namespace FullStack.Api.Application.Interfaces;

public interface ISupplierService
{
    Task<ServiceResult<ListResponse<SupplierDto>>> GetListAsync(CancellationToken ct = default);
    Task<ServiceResult<DataResponse<SupplierDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<SupplierDto>>> CreateAsync(SupplierRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<SupplierDto>>> UpdateAsync(SupplierRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
