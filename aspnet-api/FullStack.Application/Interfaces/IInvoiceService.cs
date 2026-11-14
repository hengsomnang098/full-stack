using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Invoices;

namespace FullStack.Application.Interfaces;

public interface IInvoiceService
{
    Task<ServiceResult<ListResponse<InvoiceListItemDto>>> GetListAsync(CancellationToken ct = default);
    Task<ServiceResult<ListResponse<InvoiceDetailDto>>> GetDetailsAsync(int invoiceId, CancellationToken ct = default);
}
