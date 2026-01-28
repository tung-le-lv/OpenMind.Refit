using OpenMind.Refit.OrderService.Features.Orders.PlaceOrder;

namespace OpenMind.Refit.OrderService.Features;

public static class OrdersEndpointExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPlaceOrderEndpoint();

        return app;
    }
}
