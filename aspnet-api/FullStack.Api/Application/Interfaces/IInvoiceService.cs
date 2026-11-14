using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Invoices;

namespace FullStack.Api.Application.Interfaces;

public interface IInvoiceService
{
    Task<ServiceResult<ListResponse<InvoiceListItemDto>>> GetListAsync(CancellationToken ct = default);
    Task<ServiceResult<ListResponse<InvoiceDetailDto>>> GetDetailsAsync(int invoiceId, CancellationToken ct = default);
}
