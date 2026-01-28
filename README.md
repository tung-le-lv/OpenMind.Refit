# OpenMind Order Service

Order Management Service with **Refit** integration for calling the Payment Gateway.

## Overview

This service demonstrates:
- **Refit** for type-safe HTTP client generation
- **Vertical Slice Architecture** with features organized by use case
- **Minimal APIs** with .NET 10
- Service-to-service communication using Refit

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders/place` | **Place order with payment** (calls PaymentGateway via Refit) |
| GET | `/api/orders` | Get all orders |
| GET | `/api/orders/{id}` | Get order by ID |
| POST | `/api/orders` | Create a new order |
| PUT | `/api/orders/{id}` | Update an order |
| PATCH | `/api/orders/{id}/status` | Update order status |
| DELETE | `/api/orders/{id}` | Delete an order |
| GET | `/api/customers/{customerId}/orders` | Get customer orders |

## Refit Integration

### IPaymentGatewayApi Interface

```csharp
public interface IPaymentGatewayApi
{
    // [Body] tells Refit to serialize the request object as JSON
    [Post("/api/payments/process")]
    Task<ApiResponse<ProcessPaymentResponse>> ProcessPaymentAsync([Body] ProcessPaymentRequest request);

    // Route parameters are automatically bound from method parameters
    [Get("/api/payments/{paymentId}")]
    Task<ApiResponse<PaymentResponse>> GetPaymentAsync(Guid paymentId);
}
```

### Refit Client Configuration

```csharp
services
    .AddRefitClient<IPaymentGatewayApi>(new RefitSettings
    {
        ContentSerializer = new SystemTextJsonContentSerializer(...)
    })
    .ConfigureHttpClient(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5132");
    })
    .AddHttpMessageHandler<AuthHeaderHandler>();
```

## Running the Service

```bash
# Ensure PaymentGateway is running first on port 5132
dotnet run
# Runs on http://localhost:5012
```

## Configuration

```json
{
  "PaymentGateway": {
    "BaseUrl": "http://localhost:5132"
  }
}
```

## Swagger

Open http://localhost:5012/ in your browser.
