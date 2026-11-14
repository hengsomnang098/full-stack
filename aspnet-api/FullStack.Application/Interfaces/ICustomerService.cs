using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Customers;

namespace FullStack.Application.Interfaces;

public interface ICustomerService
{
    Task<ServiceResult<ListResponse<CustomerDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<CustomerDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<CustomerDto>>> CreateAsync(CustomerRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<CustomerDto>>> UpdateAsync(CustomerRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
