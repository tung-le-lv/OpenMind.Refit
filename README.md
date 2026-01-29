## Overview

Refit is an automatic type-safe REST library for .NET that simplifies consuming RESTful APIs. It reduces the boilerplate code associated with standard HttpClient usage by allowing developers to define API endpoints as simple C# interfaces with attributes, which Refit then implements automatically at build time. 

## Reference

https://github.com/reactiveui/refit

## Setup

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
