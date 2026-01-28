# OpenMind Order Service

Order Management Service with **Refit** integration for calling the Payment Gateway.

## Overview

This service demonstrates:
- **Refit** for type-safe HTTP client generation
- **Vertical Slice Architecture** with features organized by use case
- **Minimal APIs** with .NET 10
- **EF Core In-Memory Database** for data persistence
- Service-to-service communication using Refit

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/orders/place` | **Place order with payment** (calls PaymentGateway via Refit) |
| GET | `/api/orders/{id}` | Get order by ID |
| POST | `/api/orders` | Create a new order |

## Database

This service uses **EF Core In-Memory Database** for simplicity:

```csharp
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseInMemoryDatabase("OrderDb"));
```

**Note:** No connection string is required for In-Memory database - just the database name. Data exists only in memory and is lost when the application stops. This is ideal for demo/development purposes.

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
