using FullStack.Api.Application.DTOs.Common;
using FullStack.Api.Application.DTOs.Invoices;
using FullStack.Api.Application.Interfaces;
using FullStack.Api.Application.Mapping;
using FullStack.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Application.Services;

public class InvoiceService : IInvoiceService
{
    private readonly AppDbContext _db;

    public InvoiceService(AppDbContext db) => _db = db;

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
