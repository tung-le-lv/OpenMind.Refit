using Refit;

namespace OpenMind.Refit.OrderService.ExternalApis;

/// <summary>
/// Refit interface for Payment Gateway API.
/// Refit generates the implementation automatically based on the interface definition.
/// </summary>
public interface IPaymentGatewayApi
{
    // [Body] tells Refit to serialize the request object as JSON in the request body
    [Post("/api/payments/process")]
    Task<ApiResponse<ProcessPaymentResponse>> ProcessPaymentAsync([Body] ProcessPaymentRequest request);

    // Route parameters like {paymentId} are automatically bound from method parameters
    [Get("/api/payments/{paymentId}")]
    Task<ApiResponse<PaymentResponse>> GetPaymentAsync(Guid paymentId);

    [Post("/api/payments/{paymentId}/refund")]
    Task<ApiResponse<RefundPaymentResponse>> RefundPaymentAsync(Guid paymentId, [Body] RefundPaymentRequest request);

    [Post("/api/payments/validate-card")]
    Task<ApiResponse<ValidateCardResponse>> ValidateCardAsync([Body] ValidateCardRequest request);
}

public class ProcessPaymentRequest
{
    public string OrderId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Currency { get; set; }
    public PaymentCardInfo? Card { get; set; }
    public string? PaymentMethod { get; set; }
}

public class PaymentCardInfo
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class ProcessPaymentResponse
{
    public Guid PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
}

public class PaymentResponse
{
    public Guid PaymentId { get; set; }
    public string OrderId { get; set; } = string.Empty;
    public string TransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class RefundPaymentRequest
{
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
}

public class RefundPaymentResponse
{
    public Guid RefundId { get; set; }
    public Guid PaymentId { get; set; }
    public string RefundTransactionId { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime ProcessedAt { get; set; }
}

public class ValidateCardRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class ValidateCardResponse
{
    public bool IsValid { get; set; }
    public string CardType { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public bool ExpiryValid { get; set; }
}
