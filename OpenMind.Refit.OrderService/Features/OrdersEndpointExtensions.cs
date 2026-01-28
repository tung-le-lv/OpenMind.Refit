using OpenMind.Refit.OrderService.Features.Orders.CreateOrder;
using OpenMind.Refit.OrderService.Features.Orders.DeleteOrder;
using OpenMind.Refit.OrderService.Features.Orders.GetCustomerOrders;
using OpenMind.Refit.OrderService.Features.Orders.GetOrder;
using OpenMind.Refit.OrderService.Features.Orders.GetOrders;
using OpenMind.Refit.OrderService.Features.Orders.UpdateOrder;
using OpenMind.Refit.OrderService.Features.Orders.UpdateOrderStatus;

namespace OpenMind.Refit.OrderService.Features;

public static class OrdersEndpointExtensions
{
    public static IEndpointRouteBuilder MapOrderEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetOrderEndpoint();
        app.MapGetOrdersEndpoint();
        app.MapCreateOrderEndpoint();
        app.MapUpdateOrderEndpoint();
        app.MapUpdateOrderStatusEndpoint();
        app.MapDeleteOrderEndpoint();
        app.MapGetCustomerOrdersEndpoint();

        return app;
    }
}
