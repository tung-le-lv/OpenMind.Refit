using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using OpenMind.Refit.OrderService.ExternalApis.Contracts;
using Refit;

namespace OpenMind.Refit.OrderService.Features.Orders.UpdateOrder;

public static class UpdateOrderEndpoint
{
    public static void MapUpdateOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPut("/api/orders/{id:int}", HandleAsync)
            .WithName("UpdateOrder")
            .WithTags("Orders")
            .WithSummary("Update an existing order")
            .WithDescription("Updates an order via the external API using Refit PUT method")
            .Produces<UpdateOrderResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<Results<Ok<UpdateOrderResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> HandleAsync(
        int id,
        UpdateOrderRequest request,
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

        try
        {
            var apiRequest = new ExternalApis.Contracts.UpdateOrderRequest
            {
                CustomerName = request.CustomerName,
                CustomerEmail = request.CustomerEmail,
                Status = request.Status,
                Items = request.Items.Select(i => new CreateOrderItemRequest
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            var updatedOrder = await orderApi.UpdateOrderAsync(id, apiRequest);

            return TypedResults.Ok(new UpdateOrderResponse
            {
                Id = updatedOrder.Id,
                CustomerName = updatedOrder.CustomerName,
                CustomerEmail = updatedOrder.CustomerEmail,
                TotalAmount = updatedOrder.TotalAmount,
                Status = updatedOrder.Status,
                CreatedAt = updatedOrder.CreatedAt,
                UpdatedAt = updatedOrder.UpdatedAt
            });
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Order not found",
                Detail = $"Order with ID {id} was not found",
                Status = StatusCodes.Status404NotFound
            });
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

public class UpdateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<UpdateOrderItemRequest> Items { get; set; } = [];

    public class UpdateOrderItemRequest
    {
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
    }
}

public class UpdateOrderResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
