# OpenMind Order Service - Refit Demo

A demonstration of **Refit** usage with **Vertical Slice Architecture** in ASP.NET Core Web API (.NET 10).

## Overview

This project showcases how to build a type-safe REST API client using Refit in a modern ASP.NET Core application. It follows the vertical slice architecture pattern, where each feature is self-contained in its own folder with all related code.

## Features

- **Refit Integration**: Type-safe HTTP client for consuming external REST APIs
- **Vertical Slice Architecture**: Features organized by use case, not by layer
- **Minimal APIs**: Modern .NET 10 minimal API endpoints
- **OpenAPI/Swagger**: API documentation out of the box
- **DelegatingHandler**: Custom HTTP message handler for authentication headers

## Project Structure

```
OpenMind.Refit.OrderService/
├── Domain/                          # Domain models
│   └── Order.cs
├── ExternalApis/                    # Refit API clients
│   ├── Contracts/                   # DTOs for external APIs
│   │   ├── OrderDto.cs
│   │   └── OrderRequests.cs
│   ├── AuthHeaderHandler.cs         # Custom delegating handler
│   └── IExternalOrderApi.cs         # Refit interface
├── Features/                        # Vertical slices
│   ├── Orders/
│   │   ├── CreateOrder/
│   │   │   └── CreateOrderEndpoint.cs
│   │   ├── DeleteOrder/
│   │   │   └── DeleteOrderEndpoint.cs
│   │   ├── GetCustomerOrders/
│   │   │   └── GetCustomerOrdersEndpoint.cs
│   │   ├── GetOrder/
│   │   │   └── GetOrderEndpoint.cs
│   │   ├── GetOrders/
│   │   │   └── GetOrdersEndpoint.cs
│   │   ├── UpdateOrder/
│   │   │   └── UpdateOrderEndpoint.cs
│   │   └── UpdateOrderStatus/
│   │       └── UpdateOrderStatusEndpoint.cs
│   └── OrdersEndpointExtensions.cs  # Endpoint registration
├── Infrastructure/
│   └── RefitServiceExtensions.cs    # Refit DI configuration
├── Program.cs
└── appsettings.json
```

## Refit Features Demonstrated

### 1. Basic CRUD Operations

```csharp
public interface IExternalOrderApi
{
    [Get("/orders/{id}")]
    Task<OrderDto> GetOrderAsync(int id);

    [Post("/orders")]
    Task<OrderDto> CreateOrderAsync([Body] CreateOrderRequest request);

    [Put("/orders/{id}")]
    Task<OrderDto> UpdateOrderAsync(int id, [Body] UpdateOrderRequest request);

    [Delete("/orders/{id}")]
    Task DeleteOrderAsync(int id);
}
```

### 2. Query Parameters with Object

```csharp
[Get("/orders")]
Task<List<OrderDto>> GetOrdersAsync([Query] OrderQueryParameters parameters);

public class OrderQueryParameters
{
    public int? CustomerId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
```

### 3. Route Parameters

```csharp
[Get("/customers/{customerId}/orders")]
Task<List<OrderDto>> GetCustomerOrdersAsync(int customerId);
```

### 4. ApiResponse for Response Metadata

```csharp
[Get("/orders/{id}")]
Task<ApiResponse<OrderDto>> GetOrderWithMetadataAsync(int id);

// Usage:
var response = await orderApi.GetOrderWithMetadataAsync(id);
if (response.IsSuccessStatusCode)
{
    var order = response.Content;
    var headers = response.Headers;
}
```

### 5. Dynamic Headers

```csharp
[Get("/orders")]
Task<List<OrderDto>> GetSecureOrdersAsync([Header("Authorization")] string bearerToken);

[Get("/orders")]
Task<List<OrderDto>> GetOrdersWithAuthAsync([Authorize(scheme: "Bearer")] string token);
```

### 6. DelegatingHandler for Global Headers

```csharp
public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.Add("X-Api-Key", _apiKey);
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());
        return await base.SendAsync(request, cancellationToken);
    }
}
```

## Getting Started

### Prerequisites

- .NET 10 SDK

### Running the Application

```bash
cd OpenMind.Refit.OrderService
dotnet run
```

The API will be available at:
- Swagger UI: https://localhost:5001
- API Base: https://localhost:5001/api

### Configuration

Update `appsettings.json` to configure the external API:

```json
{
  "ExternalApi": {
    "BaseUrl": "https://your-api-url.com",
    "ApiKey": "your-api-key"
  }
}
```

## API Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/orders` | Get all orders with optional filtering |
| GET | `/api/orders/{id}` | Get a single order by ID |
| POST | `/api/orders` | Create a new order |
| PUT | `/api/orders/{id}` | Update an existing order |
| PATCH | `/api/orders/{id}/status` | Update order status |
| DELETE | `/api/orders/{id}` | Delete an order |
| GET | `/api/customers/{customerId}/orders` | Get orders by customer |

## Vertical Slice Architecture

Each feature is self-contained with:
- **Endpoint definition** with route, request/response handling
- **Request model** for incoming data
- **Response model** for outgoing data
- **Handler logic** encapsulated in the endpoint class

Benefits:
- High cohesion within each feature
- Easy to understand and maintain
- Features can be added/removed independently
- Clear ownership and responsibility

## Dependencies

- [Refit](https://github.com/reactiveui/refit) - Type-safe REST library
- [Refit.HttpClientFactory](https://www.nuget.org/packages/Refit.HttpClientFactory) - HttpClientFactory integration
- [Swashbuckle.AspNetCore](https://github.com/domaindrivendev/Swashbuckle.AspNetCore) - Swagger/OpenAPI support

## License

MIT
