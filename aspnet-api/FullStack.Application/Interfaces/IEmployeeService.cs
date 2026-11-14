using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Employees;

namespace FullStack.Application.Interfaces;

/// <summary>Employee business CRUD. Authentication (login, refresh, setPassword) lives in <see cref="IAuthService"/>.</summary>
public interface IEmployeeService
{
    Task<ServiceResult<EmployeeListResponse>> GetListAsync(string? txtSearch, string? status, string? roleId, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<EmployeeDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<EmployeeCreateResponse>> CreateAsync(EmployeeRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<EmployeeDto>>> UpdateAsync(EmployeeRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
