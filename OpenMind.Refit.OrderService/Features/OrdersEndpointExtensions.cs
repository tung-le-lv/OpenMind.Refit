using OpenMind.Refit.OrderService.Features.Orders.CreateOrder;
using OpenMind.Refit.OrderService.Features.Orders.GetOrder;
using OpenMind.Refit.OrderService.Features.Orders.PlaceOrder;

namespace OpenMind.Refit.OrderService.Features;

public static class OrdersEndpointExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateOrderEndpoint();
        app.MapGetOrderEndpoint();
        app.MapPlaceOrderEndpoint();

        return app;
    }
}
