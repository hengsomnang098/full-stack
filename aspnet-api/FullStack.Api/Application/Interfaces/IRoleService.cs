using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Roles;

namespace FullStack.Api.Application.Interfaces;

public interface IRoleService
{
    Task<ServiceResult<ListResponse<RoleDto>>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<RoleDto>>> CreateAsync(RoleRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<RoleDto>>> UpdateAsync(RoleRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(string id, CancellationToken ct = default);
}
