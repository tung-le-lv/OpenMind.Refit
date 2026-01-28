using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using OpenMind.Refit.OrderService.ExternalApis.Contracts;
using Refit;

namespace OpenMind.Refit.OrderService.Features.Orders.CreateOrder;

public static class CreateOrderEndpoint
{
    public static void MapCreateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/orders", HandleAsync)
            .WithName("CreateOrder")
            .WithTags("Orders")
            .WithSummary("Create a new order")
            .WithDescription("Creates a new order via the external API using Refit")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<Results<Created<CreateOrderResponse>, BadRequest<ProblemDetails>>> HandleAsync(
        CreateOrderRequest request,
        IExternalOrderApi orderApi,
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

        try
        {
            // ApiResponse<T> allows accessing response headers (e.g., Location) after the request
            var apiRequest = new ExternalApis.Contracts.CreateOrderRequest
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Items = request.Items.Select(i => new CreateOrderItemRequest
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var response = await orderApi.CreateOrderWithMetadataAsync(apiRequest);

            if (!response.IsSuccessStatusCode || response.Content is null)
            {
                return TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Failed to create order",
                    Detail = response.Error?.Content ?? "Unknown error occurred",
                    Status = StatusCodes.Status400BadRequest
                });
            }

            var createdOrder = response.Content;
            var result = new CreateOrderResponse
            {
                Id = createdOrder.Id,
                CustomerName = createdOrder.CustomerName,
                CustomerEmail = createdOrder.CustomerEmail,
                TotalAmount = createdOrder.TotalAmount,
                Status = createdOrder.Status,
                CreatedAt = createdOrder.CreatedAt
            };

            // Access Location header from ApiResponse
            var locationHeader = response.Headers.Location?.ToString();

            return TypedResults.Created(
                locationHeader ?? $"/api/orders/{createdOrder.Id}",
                result);
        }
        catch (ApiException ex)
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "API Error",
                Detail = ex.Content ?? ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }
}

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<CreateOrderItemRequest> Items { get; set; } = [];

    public class CreateOrderItemRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

public class CreateOrderResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
