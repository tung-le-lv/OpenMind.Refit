using OpenMind.Refit.OrderService.ExternalApis.Contracts;
using Refit;

namespace OpenMind.Refit.OrderService.ExternalApis;

/// <summary>
/// Refit interface for external Order API.
/// Refit generates the implementation automatically based on the interface definition.
/// Each method maps to an HTTP endpoint using attributes like [Get], [Post], [Put], [Patch], [Delete].
/// </summary>
public interface IExternalOrderApi
{
    [Get("/orders/{id}")]
    Task<OrderDto> GetOrderAsync(int id);

    // [Query] attribute tells Refit to serialize the object as query string parameters
    [Get("/orders")]
    Task<List<OrderDto>> GetOrdersAsync([Query] OrderQueryParameters? parameters = null);

    // Route parameters are automatically bound from method parameters matching {paramName}
    [Get("/customers/{customerId}/orders")]
    Task<List<OrderDto>> GetCustomerOrdersAsync(int customerId);

    // [Body] attribute tells Refit to serialize the object as the request body
    [Post("/orders")]
    Task<OrderDto> CreateOrderAsync([Body] CreateOrderRequest request);

    [Put("/orders/{id}")]
    Task<OrderDto> UpdateOrderAsync(int id, [Body] UpdateOrderRequest request);

    [Patch("/orders/{id}")]
    Task<OrderDto> PatchOrderAsync(int id, [Body] PatchOrderRequest request);

    [Delete("/orders/{id}")]
    Task DeleteOrderAsync(int id);

    // ApiResponse<T> provides access to response metadata (status code, headers) along with deserialized content
    [Get("/orders/{id}")]
    Task<ApiResponse<OrderDto>> GetOrderWithMetadataAsync(int id);

    // ApiResponse<T> allows checking IsSuccessStatusCode and accessing Headers like Location
    [Post("/orders")]
    Task<ApiResponse<OrderDto>> CreateOrderWithMetadataAsync([Body] CreateOrderRequest request);

    // [Header] attribute passes a dynamic header value as a method parameter
    [Get("/orders")]
    Task<List<OrderDto>> GetSecureOrdersAsync([Header("Authorization")] string bearerToken);

    // [Authorize] attribute is a convenient way to add Bearer token authentication
    [Get("/orders")]
    Task<List<OrderDto>> GetOrdersWithAuthAsync([Authorize(scheme: "Bearer")] string token);
}
