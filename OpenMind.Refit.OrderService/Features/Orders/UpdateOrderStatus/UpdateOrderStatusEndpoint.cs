using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using OpenMind.Refit.OrderService.ExternalApis.Contracts;
using Refit;

namespace OpenMind.Refit.OrderService.Features.Orders.UpdateOrderStatus;

public static class UpdateOrderStatusEndpoint
{
    public static void MapUpdateOrderStatusEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPatch("/api/orders/{id:int}/status", HandleAsync)
            .WithName("UpdateOrderStatus")
            .WithTags("Orders")
            .WithSummary("Update order status")
            .WithDescription("Partially updates an order's status via the external API using Refit PATCH method")
            .Produces<UpdateOrderStatusResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound)
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static async Task<Results<Ok<UpdateOrderStatusResponse>, NotFound<ProblemDetails>, BadRequest<ProblemDetails>>> HandleAsync(
        int id,
        UpdateOrderStatusRequest request,
        IExternalOrderApi orderApi,
        CancellationToken cancellationToken)
    {
        var validStatuses = new[] { "Pending", "Confirmed", "Processing", "Shipped", "Delivered", "Cancelled" };
        if (!validStatuses.Contains(request.Status, StringComparer.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest(new ProblemDetails
            {
                Title = "Validation Error",
                Detail = $"Invalid status. Valid statuses are: {string.Join(", ", validStatuses)}",
                Status = StatusCodes.Status400BadRequest
            });
        }

        try
        {
            var patchRequest = new PatchOrderRequest
            {
                Status = request.Status
            };

            var updatedOrder = await orderApi.PatchOrderAsync(id, patchRequest);

            return TypedResults.Ok(new UpdateOrderStatusResponse
            {
                Id = updatedOrder.Id,
                Status = updatedOrder.Status,
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

public class UpdateOrderStatusRequest
{
    public string Status { get; set; } = string.Empty;
}

public class UpdateOrderStatusResponse
{
    public int Id { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime? UpdatedAt { get; set; }
}
