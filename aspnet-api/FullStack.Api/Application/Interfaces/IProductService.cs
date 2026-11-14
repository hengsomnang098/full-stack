using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Products;

namespace FullStack.Api.Application.Interfaces;

public interface IProductService
{
    Task<ServiceResult<ProductListResponse>> GetListAsync(string? txtSearch, string? status, string? categoryId, CancellationToken ct = default);
    Task<ServiceResult<DataResponse<ProductDto>>> GetByIdAsync(int id, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<ProductDto>>> CreateAsync(ProductRequest request, IFormFile? image, CancellationToken ct = default);
    Task<ServiceResult<MessageDataResponse<ProductDto>>> UpdateAsync(ProductRequest request, IFormFile? image, CancellationToken ct = default);
    Task<ServiceResult<ApiMessageResponse>> DeleteAsync(int id, CancellationToken ct = default);
}
