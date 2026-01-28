using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenMind.Refit.PaymentGateway.Features.Payments.GetPayment;

public static class GetPaymentEndpoint
{
    private static readonly Dictionary<Guid, PaymentResponse> PaymentsStore = new();

    public static void MapGetPaymentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/payments/{paymentId:guid}", HandleAsync)
            .WithName("GetPayment")
            .WithTags("Payments")
            .WithSummary("Get payment by ID")
            .WithDescription("Retrieves payment details by payment ID")
            .Produces<PaymentResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Task<Results<Ok<PaymentResponse>, NotFound<ProblemDetails>>> HandleAsync(
        Guid paymentId,
        CancellationToken cancellationToken)
    {
        // Return a mock payment for demo purposes
        var payment = new PaymentResponse
        {
            PaymentId = paymentId,
            OrderId = $"ORD-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            TransactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            Amount = 99.99m,
            Currency = "USD",
            Status = "Completed",
            PaymentMethod = "CreditCard",
            CreatedAt = DateTime.UtcNow.AddMinutes(-5),
            ProcessedAt = DateTime.UtcNow.AddMinutes(-4)
        };

        return Task.FromResult<Results<Ok<PaymentResponse>, NotFound<ProblemDetails>>>(
            TypedResults.Ok(payment));
    }
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
