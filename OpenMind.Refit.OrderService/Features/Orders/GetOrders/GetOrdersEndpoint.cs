using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using OpenMind.Refit.OrderService.ExternalApis.Contracts;

namespace OpenMind.Refit.OrderService.Features.Orders.GetOrders;

public static class GetOrdersEndpoint
{
    public static void MapGetOrdersEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("/api/orders", HandleAsync)
            .WithName("GetOrders")
            .WithTags("Orders")
            .WithSummary("Get all orders")
            .WithDescription("Retrieves all orders with optional filtering using query parameters")
            .Produces<GetOrdersResponse>();
    }

    private static async Task<Ok<GetOrdersResponse>> HandleAsync(
        [AsParameters] GetOrdersRequest request,
        IExternalOrderApi orderApi,
        CancellationToken cancellationToken)
    {
        // Refit automatically serializes OrderQueryParameters as query string using [Query] attribute
        var queryParams = new OrderQueryParameters
        {
            CustomerId = request.CustomerId,
            Status = request.Status,
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            Page = request.Page,
            PageSize = request.PageSize
        };

        var orders = await orderApi.GetOrdersAsync(queryParams);

        var response = new GetOrdersResponse
        {
            Orders = orders.Select(o => new GetOrdersResponse.OrderSummary
            {
                Id = o.Id,
                CustomerName = o.CustomerName,
                TotalAmount = o.TotalAmount,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                ItemCount = o.Items.Count
            }).ToList(),
            TotalCount = orders.Count,
            Page = request.Page ?? 1,
            PageSize = request.PageSize ?? 10
        };

        return TypedResults.Ok(response);
    }
}

public class GetOrdersRequest
{
    public int? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public int? Page { get; set; }
    public int? PageSize { get; set; }
}

public class GetOrdersResponse
{
    public List<OrderSummary> Orders { get; set; } = [];
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }

    public class OrderSummary
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int ItemCount { get; set; }
    }
}
