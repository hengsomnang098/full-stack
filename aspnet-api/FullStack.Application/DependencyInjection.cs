using FullStack.Application.Interfaces;
using FullStack.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FullStack.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ICategoryService, CategoryService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<ISupplierService, SupplierService>();
        services.AddScoped<IOrderStatusService, OrderStatusService>();
        services.AddScoped<IOrderPaymentMethodService, OrderPaymentMethodService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<IInvoiceService, InvoiceService>();
        services.AddScoped<IPosService, PosService>();
        return services;
    }
}
