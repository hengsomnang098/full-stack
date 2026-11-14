using System.Text.Json.Serialization;
using FullStack.Api.Application.DTOs.Categories;
using FullStack.Api.Application.DTOs.Customers;
using FullStack.Api.Application.DTOs.PaymentMethods;
using FullStack.Api.Application.DTOs.Products;

namespace FullStack.Api.Application.DTOs.Pos;

/// <summary>Express shape: <c>{ order_payment_method: [...], customer: [...] }</c>.</summary>
public class PosInitResponse
{
    [JsonPropertyName("order_payment_method")]
    public IReadOnlyList<OrderPaymentMethodDto> OrderPaymentMethod { get; set; } = Array.Empty<OrderPaymentMethodDto>();

    [JsonPropertyName("customer")]
    public IReadOnlyList<CustomerDto> Customer { get; set; } = Array.Empty<CustomerDto>();
}

/// <summary>Express shape: <c>{ list: [...], category: [...] }</c>.</summary>
public class PosSearchResponse
{
    [JsonPropertyName("list")]
    public IReadOnlyList<ProductDto> List { get; set; } = Array.Empty<ProductDto>();

    [JsonPropertyName("category")]
    public IReadOnlyList<CategoryDto> Category { get; set; } = Array.Empty<CategoryDto>();
}

public class PosCheckoutRequest
{
    public int? CustomerId { get; set; }
    public int? PaymentMethodId { get; set; }
    public decimal? TotalPaid { get; set; }

    /// <summary>Cart lines. Only Id and QtyOrder are used; prices and discounts are always read from the database.</summary>
    public List<PosCheckoutItem>? Product { get; set; }
}

public class PosCheckoutItem
{
    public int? Id { get; set; }
    public int? QtyOrder { get; set; }
}

public class PosCheckoutResult
{
    public int InvoiceId { get; set; }
    public decimal TotalQty { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalPaid { get; set; }
    public int? OrderStatusId { get; set; }
}
