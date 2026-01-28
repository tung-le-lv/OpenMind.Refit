using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using Refit;

namespace OpenMind.Refit.OrderService.Features.Orders.PlaceOrder;

public static class PlaceOrderEndpoint
{
    public static void MapPlaceOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders/place", HandleAsync)
            .WithName("PlaceOrder")
            .WithTags("Orders")
            .WithSummary("Place a new order with payment")
            .WithDescription("Creates an order and processes payment via PaymentGateway using Refit")
            .Produces<PlaceOrderResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces<ProblemDetails>(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<Results<Created<PlaceOrderResponse>, BadRequest<ProblemDetails>, UnprocessableEntity<ProblemDetails>>> HandleAsync(
        PlaceOrderRequest request,
        IPaymentGatewayApi paymentGateway,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CustomerName))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "Customer name is required",
                Status = StatusCodes.Status400BadRequest
            });
        }

        if (request.Items.Count == 0)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = "At least one order item is required",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var orderId = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}";
        var totalAmount = request.Items.Sum(i => i.Quantity * i.UnitPrice);

        // Call PaymentGateway API via Refit to process payment
        var paymentRequest = new ProcessPaymentRequest
        {
            OrderId = orderId,
            Amount = totalAmount,
            Currency = request.Currency ?? "USD",
            PaymentMethod = request.PaymentMethod,
            Card = request.Card != null ? new PaymentCardInfo
            {
                CardNumber = request.Card.CardNumber,
                CardHolderName = request.Card.CardHolderName,
                ExpiryMonth = request.Card.ExpiryMonth,
                ExpiryYear = request.Card.ExpiryYear,
                Cvv = request.Card.Cvv
            } : null
        };

        // ApiResponse<T> provides access to HTTP status, headers, and deserialized content
        var paymentResponse = await paymentGateway.ProcessPaymentAsync(paymentRequest);

        if (!paymentResponse.IsSuccessStatusCode)
        {
            // Handle payment failure - Refit wraps error details in ApiResponse
            var errorContent = paymentResponse.Error?.Content ?? "Payment processing failed";
            
            return TypedResults.UnprocessableEntity(new ProblemDetails
            {
                Title = "Payment Failed",
                Detail = errorContent,
                Status = StatusCodes.Status422UnprocessableEntity
            });
        }

        var payment = paymentResponse.Content!;

        var response = new PlaceOrderResponse
        {
            OrderId = orderId,
            CustomerName = request.CustomerName,
            CustomerEmail = request.CustomerEmail,
            TotalAmount = totalAmount,
            Currency = payment.Currency,
            Status = "Confirmed",
            PaymentId = payment.PaymentId,
            TransactionId = payment.TransactionId,
            PaymentStatus = payment.Status,
            Items = request.Items.Select(i => new PlaceOrderResponse.OrderItemResponse
            {
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList(),
            CreatedAt = DateTime.UtcNow
        };

        return TypedResults.Created($"/api/orders/{orderId}", response);
    }
}

public class PlaceOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderItemRequest> Items { get; set; } = [];
    public string? Currency { get; set; }
    public string? PaymentMethod { get; set; }
    public CardInfoRequest? Card { get; set; }
}

public class OrderItemRequest
{
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

public class CardInfoRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public string CardHolderName { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class PlaceOrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public Guid PaymentId { get; set; }
    public string TransactionId { get; set; } = string.Empty;
    public string PaymentStatus { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = [];
    public DateTime CreatedAt { get; set; }

    public class OrderItemResponse
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
