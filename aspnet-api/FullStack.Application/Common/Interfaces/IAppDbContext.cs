using FullStack.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace FullStack.Application.Common.Interfaces;

public interface IAppDbContext
{
    DbSet<Category> Categories { get; }
    DbSet<Customer> Customers { get; }
    DbSet<Employee> Employees { get; }
    DbSet<Product> Products { get; }
    DbSet<Supplier> Suppliers { get; }
    DbSet<OrderStatus> OrderStatuses { get; }
    DbSet<OrderPaymentMethod> OrderPaymentMethods { get; }
    DbSet<Invoice> Invoices { get; }
    DbSet<InvoiceDetail> InvoiceDetails { get; }

    DatabaseFacade Database { get; }

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
