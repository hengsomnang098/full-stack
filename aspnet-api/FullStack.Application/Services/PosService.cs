using FullStack.Application.Common;
using FullStack.Application.DTOs.Common;
using FullStack.Application.DTOs.Pos;
using FullStack.Application.Interfaces;
using FullStack.Application.Mapping;
using FullStack.Domain.Entities;
using FullStack.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Application.Services;

public class PosService : IPosService
{
    // Express hard-coded these order_status ids: 3 = not fully paid, 4 = fully paid.
    // They are only applied when a matching order_status row exists (otherwise OrderStatusId stays null).
    private const int PartiallyPaidStatusId = 3;
    private const int PaidStatusId = 4;

    private readonly IAppDbContext _db;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<PosService> _logger;

    public PosService(IAppDbContext db, ICurrentUserService currentUser, ILogger<PosService> logger)
    {
        _db = db;
        _currentUser = currentUser;
        _logger = logger;
    }

    public async Task<ServiceResult<PosInitResponse>> GetInitInfoAsync(CancellationToken ct = default)
    {
        var methods = await _db.OrderPaymentMethods.AsNoTracking().OrderBy(m => m.Id).ToListAsync(ct);
        var customers = await _db.Customers.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);

        return ServiceResult<PosInitResponse>.Ok(new PosInitResponse
        {
            OrderPaymentMethod = methods.Select(OrderPaymentMethodMapper.ToDto).ToList(),
            Customer = customers.Select(CustomerMapper.ToDto).ToList()
        });
    }

    public async Task<ServiceResult<PosSearchResponse>> SearchProductAsync(string? txtSearch, CancellationToken ct = default)
    {
        var search = LegacyParse.Text(txtSearch);
        if (search is null)
            return ServiceResult<PosSearchResponse>.Ok(new PosSearchResponse());

        var products = await _db.Products.AsNoTracking()
            .Include(p => p.Category)
            .Where(p => p.Name.Contains(search))
            .OrderBy(p => p.Id)
            .ToListAsync(ct);
        var categories = await _db.Categories.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);

        return ServiceResult<PosSearchResponse>.Ok(new PosSearchResponse
        {
            List = products.Select(ProductMapper.ToDto).ToList(),
            Category = categories.Select(CategoryMapper.ToDto).ToList()
        });
    }

    public async Task<ServiceResult<MessageDataResponse<PosCheckoutResult>>> CheckoutAsync(PosCheckoutRequest request, CancellationToken ct = default)
    {
        var errors = new Dictionary<string, string>();
        if (request.CustomerId is null) errors["CustomerId"] = "CustomerId required!";
        if (request.PaymentMethodId is null) errors["PaymentMethodId"] = "PaymentMethodId required!";

        var items = request.Product ?? new List<PosCheckoutItem>();
        if (items.Count == 0)
            errors["Product"] = "Please Add Product to order!";
        else if (items.Any(i => i.Id is null || i.QtyOrder is null or <= 0))
            errors["Product"] = "Every product needs an Id and a QtyOrder greater than 0.";

        if (errors.Count > 0)
            return ServiceResult<MessageDataResponse<PosCheckoutResult>>.FailValidation(errors);

        // Merge duplicate cart lines for the same product.
        var wanted = items
            .GroupBy(i => i.Id!.Value)
            .ToDictionary(g => g.Key, g => g.Sum(i => i.QtyOrder!.Value));

        await using var tx = await _db.Database.BeginTransactionAsync(ct);

        if (!await _db.Customers.AnyAsync(c => c.Id == request.CustomerId, ct))
            return Fail("CustomerId does not exist.", 400);
        if (!await _db.OrderPaymentMethods.AnyAsync(m => m.Id == request.PaymentMethodId, ct))
            return Fail("PaymentMethodId does not exist.", 400);

        var ids = wanted.Keys.ToList();
        var products = await _db.Products.AsNoTracking().Where(p => ids.Contains(p.Id)).ToListAsync(ct);
        var missing = ids.Except(products.Select(p => p.Id)).ToList();
        if (missing.Count > 0)
            return Fail($"Product not found: {string.Join(", ", missing)}.", 400);

        // Prices and discounts always come from the database, never from the client.
        decimal totalQty = 0, totalAmount = 0;
        var details = new List<InvoiceDetail>();
        foreach (var p in products)
        {
            var qty = wanted[p.Id];
            var price = p.Price ?? 0m;
            var discount = p.Discount ?? 0m;
            var lineGross = qty * price;
            var lineTotal = lineGross - lineGross * discount / 100m;

            totalQty += qty;
            totalAmount += lineTotal;
            details.Add(new InvoiceDetail { ProductId = p.Id, Qty = qty, Price = price, Discount = discount });
        }
        totalAmount = Math.Round(totalAmount, 2, MidpointRounding.AwayFromZero);

        var totalPaid = request.TotalPaid ?? 0m;
        var statusId = await ResolveOrderStatusIdAsync(totalPaid < totalAmount ? PartiallyPaidStatusId : PaidStatusId, ct);

        var employeeId = _currentUser.EmployeeId;
        if (employeeId is null)
            _logger.LogWarning("POS checkout by a user without an employee_id claim; invoice EmployeeId will be null.");

        var invoice = new Invoice
        {
            CustomerId = request.CustomerId,
            EmployeeId = employeeId,
            OrderStatusId = statusId,
            OrderPaymentMethodId = request.PaymentMethodId,
            TotalQty = totalQty,
            TotalAmount = totalAmount,
            TotalPaid = totalPaid,
            CreateBy = employeeId,
            Details = details
        };
        _db.Invoices.Add(invoice);

        try
        {
            await _db.SaveChangesAsync(ct);

            // Atomic, race-safe stock decrement: the row only updates while enough stock remains.
            foreach (var p in products)
            {
                var qty = wanted[p.Id];
                var affected = await _db.Products
                    .Where(x => x.Id == p.Id && x.Qty >= qty)
                    .ExecuteUpdateAsync(s => s.SetProperty(x => x.Qty, x => x.Qty - qty), ct);

                if (affected == 0)
                {
                    await tx.RollbackAsync(ct);
                    return Fail($"Insufficient stock for product '{p.Name}'.", 400);
                }
            }

            await tx.CommitAsync(ct);
        }
        catch (DbUpdateException ex) when (DbErrorMapper.Map(ex) is { } mapped)
        {
            _logger.LogWarning(ex, "POS checkout failed");
            return Fail(mapped.Message, mapped.StatusCode);
        }

        _logger.LogInformation("POS invoice {InvoiceId} created by employee {EmployeeId}", invoice.Id, employeeId);

        return ServiceResult<MessageDataResponse<PosCheckoutResult>>.Ok(
            MessageDataResponse<PosCheckoutResult>.From("Order Success", new PosCheckoutResult
            {
                InvoiceId = invoice.Id,
                TotalQty = totalQty,
                TotalAmount = totalAmount,
                TotalPaid = totalPaid,
                OrderStatusId = statusId
            }));

        static ServiceResult<MessageDataResponse<PosCheckoutResult>> Fail(string message, int status) =>
            ServiceResult<MessageDataResponse<PosCheckoutResult>>.Fail(message, status);
    }

    private async Task<int?> ResolveOrderStatusIdAsync(int preferredId, CancellationToken ct) =>
        await _db.OrderStatuses.AnyAsync(s => s.Id == preferredId, ct) ? preferredId : null;
}
