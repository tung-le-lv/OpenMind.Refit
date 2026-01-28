using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;

namespace OpenMind.Refit.OrderService.Features.Orders.GetCustomerOrders;

public static class GetCustomerOrdersEndpoint
{
    public static void MapGetCustomerOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/customers/{customerId:int}/orders", HandleAsync)
            .WithName("GetCustomerOrders")
            .WithTags("Orders")
            .WithSummary("Get orders by customer")
            .WithDescription("Retrieves all orders for a specific customer using route parameter binding")
            .Produces<GetCustomerOrdersResponse>();
    }

    private static async Task<Ok<GetCustomerOrdersResponse>> HandleAsync(
        int customerId,
        IExternalOrderApi orderApi,
        CancellationToken cancellationToken)
    {
        // Route parameters like {customerId} are automatically bound by Refit
        var orders = await orderApi.GetCustomerOrdersAsync(customerId);

        return TypedResults.Ok(new GetCustomerOrdersResponse
        {
            CustomerId = customerId,
            Orders = orders.Select(o => new GetCustomerOrdersResponse.CustomerOrderSummary
            {
                Id = o.Id,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ItemCount = o.Items.Count
            }).ToList(),
            TotalOrders = orders.Count,
            TotalSpent = orders.Sum(o => o.TotalAmount)
        });
    }
}

public class GetCustomerOrdersResponse
{
    public int CustomerId { get; set; }
    public List<CustomerOrderSummary> Orders { get; set; } = [];
    public int TotalOrders { get; set; }
    public decimal TotalSpent { get; set; }

    public class CustomerOrderSummary
    {
        public int Id { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }
}
