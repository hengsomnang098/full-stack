using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Suppliers;

namespace FullStack.Application.Interfaces;

public interface ISupplierService
{
    Task<ServiceResult<ListResponse<SupplierDto>>> GetListAsync(CancellationToken ct = default);
    Task<ServiceResult<DataResponse<SupplierDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<SupplierDto>>> CreateAsync(SupplierRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<SupplierDto>>> UpdateAsync(SupplierRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
