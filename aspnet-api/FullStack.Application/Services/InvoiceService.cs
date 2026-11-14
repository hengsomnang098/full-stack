using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Invoices;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IAppDbContext _db;

    public InvoiceService(IAppDbContext db) => _db = db;

    public async Task<ServiceResult<ListResponse<InvoiceListItemDto>>> GetListAsync(CancellationToken ct = default)
    {
        var items = await _db.Invoices.AsNoTracking()
            .Include(i => i.Customer)
            .Include(i => i.Employee)
            .Include(i => i.OrderPaymentMethod)
            .Include(i => i.OrderStatus)
            .OrderByDescending(i => i.Id)
            .ToListAsync(ct);

        return ServiceResult<ListResponse<InvoiceListItemDto>>.Ok(
            ListResponse<InvoiceListItemDto>.From(items.Select(InvoiceMapper.ToListItem).ToList()));
    }

    public async Task<ServiceResult<ListResponse<InvoiceDetailDto>>> GetDetailsAsync(int invoiceId, CancellationToken ct = default)
    {
        var items = await _db.InvoiceDetails.AsNoTracking()
            .Include(d => d.Product)
            .Where(d => d.InvoiceId == invoiceId)
            .OrderBy(d => d.Id)
            .ToListAsync(ct);

        return ServiceResult<ListResponse<InvoiceDetailDto>>.Ok(
            ListResponse<InvoiceDetailDto>.From(items.Select(InvoiceMapper.ToDetailDto).ToList()));
    }
}
