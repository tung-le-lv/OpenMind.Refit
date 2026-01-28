# OpenMind Payment Gateway

Payment Processing Service that provides APIs for processing payments, refunds, and card validation.

## Overview

This service provides payment-related endpoints that are consumed by the Order Service using Refit.

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/payments/process` | Process a payment |
| GET | `/api/payments/{paymentId}` | Get payment by ID |
| POST | `/api/payments/{paymentId}/refund` | Refund a payment |
| POST | `/api/payments/validate-card` | Validate card details |

## Running the Service

```bash
dotnet run
# Runs on http://localhost:5132
```

## Swagger

Open http://localhost:5132/ in your browser.

## Example: Process Payment

```bash
curl -X POST http://localhost:5132/api/payments/process \
  -H "Content-Type: application/json" \
  -d '{
    "orderId": "ORD-001",
    "amount": 99.99,
    "currency": "USD",
    "paymentMethod": "CreditCard",
    "card": {
      "cardNumber": "4111111111111111",
      "cardHolderName": "John Doe",
      "expiryMonth": "12",
      "expiryYear": "2025",
      "cvv": "123"
    }
  }'
```

## Response Example

```json
{
  "paymentId": "959869ca-7c74-4d9a-a5a6-5d6846a3db65",
  "orderId": "ORD-001",
  "transactionId": "TXN-20240128-8A203BA5",
  "amount": 99.99,
  "currency": "USD",
  "status": "Completed",
  "processedAt": "2024-01-28T16:04:14.959995Z"
}
```
