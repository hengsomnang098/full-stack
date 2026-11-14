using FullStack.Api.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FullStack.Api.Infrastructure.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Employee> Employees => Set<Employee>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<Supplier> Suppliers => Set<Supplier>();
    public DbSet<OrderStatus> OrderStatuses => Set<OrderStatus>();
    public DbSet<OrderPaymentMethod> OrderPaymentMethods => Set<OrderPaymentMethod>();
    public DbSet<Invoice> Invoices => Set<Invoice>();
    public DbSet<InvoiceDetail> InvoiceDetails => Set<InvoiceDetail>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        b.Entity<ApplicationUser>(e =>
        {
            e.Property(x => x.FirstName).HasMaxLength(120);
            e.Property(x => x.LastName).HasMaxLength(120);
            e.Property(x => x.Tel).HasMaxLength(120);
            e.Property(x => x.Image).HasMaxLength(255);
            e.Property(x => x.Salary).HasPrecision(10, 2);
            e.HasIndex(x => x.Tel).IsUnique();
        });

        b.Entity<Category>(e =>
        {
            e.ToTable("category");
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.Property(x => x.Description).HasColumnType("text");
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<Customer>(e =>
        {
            e.ToTable("customer");
            e.Property(x => x.Firstname).HasMaxLength(120).IsRequired();
            e.Property(x => x.Lastname).HasMaxLength(120).IsRequired();
            e.Property(x => x.Tel).HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120);
            e.Property(x => x.Address).HasColumnType("text");
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<Employee>(e =>
        {
            e.ToTable("employee");
            e.Property(x => x.Firstname).HasMaxLength(120).IsRequired();
            e.Property(x => x.Lastname).HasMaxLength(120).IsRequired();
            e.Property(x => x.Tel).HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120);
            e.Property(x => x.Address).HasColumnType("text");
            e.Property(x => x.Image).HasMaxLength(255);
            e.Property(x => x.Salary).HasPrecision(6, 2);
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<Supplier>(e =>
        {
            e.ToTable("supplier");
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.Tel).HasMaxLength(120).IsRequired();
            e.Property(x => x.Email).HasMaxLength(120).IsRequired();
            e.Property(x => x.Address).HasColumnType("text").IsRequired();
            e.Property(x => x.WebsiteUrl).HasColumnType("text");
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<Product>(e =>
        {
            e.ToTable("product");
            e.Property(x => x.Name).HasMaxLength(255).IsRequired();
            e.Property(x => x.Description).HasColumnType("text");
            e.Property(x => x.Price).HasPrecision(7, 2);
            e.Property(x => x.Discount).HasPrecision(7, 2);
            e.Property(x => x.Image).HasMaxLength(255);
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.HasOne(x => x.Category).WithMany(c => c.Products).HasForeignKey(x => x.CategoryId);
        });

        b.Entity<OrderStatus>(e =>
        {
            e.ToTable("order_status");
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.Code).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<OrderPaymentMethod>(e =>
        {
            e.ToTable("order_payment_method");
            e.Property(x => x.Name).HasMaxLength(120).IsRequired();
            e.Property(x => x.Code).HasMaxLength(120).IsRequired();
            e.HasIndex(x => x.Name).IsUnique();
            e.HasIndex(x => x.Code).IsUnique();
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
        });

        b.Entity<Invoice>(e =>
        {
            e.ToTable("invoice");
            e.Property(x => x.TotalQty).HasPrecision(7, 2);
            e.Property(x => x.TotalAmount).HasPrecision(7, 2);
            e.Property(x => x.TotalPaid).HasPrecision(7, 2);
            e.Property(x => x.Note).HasColumnType("text");
            e.Property(x => x.CreateAt).HasDefaultValueSql("NOW()").ValueGeneratedOnAdd();
            e.HasOne(x => x.Customer).WithMany(c => c.Invoices).HasForeignKey(x => x.CustomerId);
            e.HasOne(x => x.Employee).WithMany(c => c.Invoices).HasForeignKey(x => x.EmployeeId);
            e.HasOne(x => x.OrderStatus).WithMany().HasForeignKey(x => x.OrderStatusId);
            e.HasOne(x => x.OrderPaymentMethod).WithMany().HasForeignKey(x => x.OrderPaymentMethodId);
        });

        b.Entity<InvoiceDetail>(e =>
        {
            e.ToTable("invoice_details");
            e.Property(x => x.Price).HasPrecision(7, 2);
            // Discount is stored as a percentage (e.g. 10 = 10%), so it needs more room than the legacy DECIMAL(3,2).
            e.Property(x => x.Discount).HasPrecision(7, 2);
            e.HasOne(x => x.Invoice).WithMany(i => i.Details).HasForeignKey(x => x.InvoiceId);
            e.HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        });
    }
}

