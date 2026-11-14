using FullStack.Application.DTOs.Categories;
using FullStack.Application.DTOs.Common;

namespace FullStack.Application.Interfaces;

public interface ICategoryService
{
    Task<ServiceResult<CategoryListResponse>> GetListAsync(string? txtSearch, string? status, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<CategoryDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<CategoryDto>>> CreateAsync(CategoryRequest request, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<CategoryDto>>> UpdateAsync(CategoryRequest request, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
