using System.Text.Json.Serialization;
using FullStack.Application.DTOs.Common;

namespace FullStack.Application.DTOs.PaymentMethods;

public class OrderPaymentMethodDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
    public DateTime CreateAt { get; set; }
    public int? CreateBy { get; set; }
    public DateTime? UpdateAt { get; set; }
    public int? UpdateBy { get; set; }
}

public class OrderPaymentMethodRequest
{
    public int? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }

    [JsonConverter(typeof(LegacyBoolConverter))]
    public bool? Status { get; set; }
}
