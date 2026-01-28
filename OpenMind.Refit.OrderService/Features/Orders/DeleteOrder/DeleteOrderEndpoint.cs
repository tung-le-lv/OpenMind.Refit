using Microsoft.AspNetCore.Http.HttpResults;
using OpenMind.Refit.OrderService.ExternalApis;
using Refit;

namespace OpenMind.Refit.OrderService.Features.Orders.DeleteOrder;

public static class DeleteOrderEndpoint
{
    public static void MapDeleteOrderEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapDelete("/api/orders/{id:int}", HandleAsync)
            .WithName("DeleteOrder")
            .WithTags("Orders")
            .WithSummary("Delete an order")
            .WithDescription("Deletes an order via the external API using Refit DELETE method")
            .Produces(StatusCodes.Status204NoContent)
            .Produces<ProblemDetails>(StatusCodes.Status404NotFound);
    }

    private static async Task<Results<NoContent, NotFound<ProblemDetails>>> HandleAsync(
        int id,
        IExternalOrderApi orderApi,
        CancellationToken cancellationToken)
    {
        try
        {
            await orderApi.DeleteOrderAsync(id);
            return TypedResults.NoContent();
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
    }
}
