using System.Text.Json.Serialization;

namespace OpenMind.Refit.OrderService.ExternalApis.Contracts;

public class CreateOrderRequest
{
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];
}

public class CreateOrderItemRequest
{
    [JsonPropertyName("productName")]
    public string ProductName { get; set; } = string.Empty;

    [JsonPropertyName("quantity")]
    public int Quantity { get; set; }

    [JsonPropertyName("unitPrice")]
    public decimal UnitPrice { get; set; }
}

public class UpdateOrderRequest
{
    [JsonPropertyName("customerName")]
    public string CustomerName { get; set; } = string.Empty;

    [JsonPropertyName("customerEmail")]
    public string CustomerEmail { get; set; } = string.Empty;

    [JsonPropertyName("items")]
    public List<CreateOrderItemRequest> Items { get; set; } = [];

    [JsonPropertyName("status")]
    public string Status { get; set; } = string.Empty;
}

public class PatchOrderRequest
{
    [JsonPropertyName("status")]
    public string? Status { get; set; }

    [JsonPropertyName("customerEmail")]
    public string? CustomerEmail { get; set; }
}

// Query parameters object - Refit serializes this as query string when using [Query] attribute
public class OrderQueryParameters
{
    public int? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}
