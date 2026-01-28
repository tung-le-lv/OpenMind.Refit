using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using OpenMind.Refit.OrderService.Domain;
using OpenMind.Refit.OrderService.Infrastructure;

namespace OpenMind.Refit.OrderService.Features.Orders.GetOrder;

public static class GetOrderEndpoint
{
    public static void MapGetOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders/{id:int}", HandleAsync)
            .WithName("GetOrder")
            .WithTags("Orders")
            .WithSummary("Get order by ID")
            .WithDescription("Retrieves a single order from the database")
            .Produces<GetOrderResponse>()
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<Ok<GetOrderResponse>, NotFound<ProblemDetails>>> HandleAsync(
        int id,
        IOrderRepository orderRepository,
        CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetByIdAsync(id, cancellationToken);

        if (order is null)
        {
            return TypedResults.NotFound(new ProblemDetails
            {
                Title = "Order not found",
                Detail = $"Order with ID {id} was not found",
                Status = StatusCodes.Status404NotFound
            });
        }

        return TypedResults.Ok(new GetOrderResponse
        {
            Id = order.Id,
            CustomerName = order.CustomerName,
            CustomerEmail = order.CustomerEmail,
            Items = order.Items.Select(i => new GetOrderResponse.OrderItemResponse
            {
                Id = i.Id,
                ProductName = i.ProductName,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice,
                TotalPrice = i.Quantity * i.UnitPrice
            }).ToList(),
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            UpdatedAt = order.UpdatedAt
        });
    }
}

public class GetOrderResponse
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string CustomerEmail { get; set; } = string.Empty;
    public List<OrderItemResponse> Items { get; set; } = [];
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public class OrderItemResponse
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
