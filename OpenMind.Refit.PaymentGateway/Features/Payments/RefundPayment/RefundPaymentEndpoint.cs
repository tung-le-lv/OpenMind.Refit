using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenMind.Refit.PaymentGateway.Features.Payments.RefundPayment;

public static class RefundPaymentEndpoint
{
    public static void MapRefundPaymentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/{paymentId:guid}/refund", HandleAsync)
            .WithName("RefundPayment")
            .WithTags("Payments")
            .WithSummary("Refund a payment")
            .WithDescription("Initiates a refund for a completed payment")
            .Produces<RefundPaymentResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<RefundPaymentResponse>, BadRequest<ProblemDetails>, NotFound<ProblemDetails>>> HandleAsync(
        Guid paymentId,
        RefundPaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Refund amount must be greater than zero",
                Status = StatusCodes.Status400BadRequest
            });
        }

        await Task.Delay(50, cancellationToken); // Simulate processing

        var refundId = Guid.NewGuid();
        var refundTransactionId = $"REF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        return TypedResults.Ok(new RefundPaymentResponse
        {
            RefundId = refundId,
            PaymentId = paymentId,
            RefundTransactionId = refundTransactionId,
            Amount = request.Amount,
            Status = "Completed",
            Reason = request.Reason,
            ProcessedAt = DateTime.UtcNow
        });
    }
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
