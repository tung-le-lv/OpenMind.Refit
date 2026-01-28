using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenMind.Refit.PaymentGateway.Features.Payments.ProcessPayment;

public static class ProcessPaymentEndpoint
{
    public static void MapProcessPaymentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/process", HandleAsync)
            .WithName("ProcessPayment")
            .WithTags("Payments")
            .WithSummary("Process a payment")
            .WithDescription("Processes a payment for an order")
            .Produces<ProcessPaymentResponse>(StatusCodes.Status200OK)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<Results<Ok<ProcessPaymentResponse>, BadRequest<ProblemDetails>, UnprocessableEntity<ProblemDetails>>> HandleAsync(
        ProcessPaymentRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.OrderId))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Order ID is required",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (request.Amount <= 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Amount must be greater than zero",
                Status = StatusCodes.Status400BadRequest
            });
        }

        await Task.Delay(100, cancellationToken); // Simulate processing

        // Simulate payment failure for amounts over 10000
        if (request.Amount > 10000)
        {
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Payment Failed",
                Detail = "Payment declined: Amount exceeds limit",
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }

        var paymentId = Guid.NewGuid();
        var transactionId = $"TXN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";

        return TypedResults.Ok(new ProcessPaymentResponse
        {
            PaymentId = paymentId,
            OrderId = request.OrderId,
            TransactionId = transactionId,
            Amount = request.Amount,
            Currency = request.Currency ?? "USD",
            Status = "Completed",
            ProcessedAt = DateTime.UtcNow
        });
    }
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
