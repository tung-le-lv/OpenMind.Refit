using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace OpenMind.Refit.PaymentGateway.Features.Payments.ValidateCard;

public static class ValidateCardEndpoint
{
    public static void MapValidateCardEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/payments/validate-card", HandleAsync)
            .WithName("ValidateCard")
            .WithTags("Payments")
            .WithSummary("Validate a payment card")
            .WithDescription("Validates card details without processing a payment")
            .Produces<ValidateCardResponse>()
            .Produces<ProblemDetails>(StatusCodes.Status400BadRequest);
    }

    private static Task<Results<Ok<ValidateCardResponse>, BadRequest<ProblemDetails>>> HandleAsync(
        ValidateCardRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.CardNumber) || request.CardNumber.Length < 13)
        {
            return Task.FromResult<Results<Ok<ValidateCardResponse>, BadRequest<ProblemDetails>>>(
                TypedResults.BadRequest(new ProblemDetails
                {
                    Title = "Validation Error",
                    Detail = "Invalid card number",
                    Status = StatusCodes.Status400BadRequest
                }));
        }

        var cardType = request.CardNumber[0] switch
        {
            '4' => "Visa",
            '5' => "Mastercard",
            '3' => "American Express",
            '6' => "Discover",
            _ => "Unknown"
        };

        return Task.FromResult<Results<Ok<ValidateCardResponse>, BadRequest<ProblemDetails>>>(
            TypedResults.Ok(new ValidateCardResponse
            {
                IsValid = true,
                CardType = cardType,
                Last4Digits = request.CardNumber[^4..],
                ExpiryValid = true
            }));
    }
}

public class ValidateCardRequest
{
    public string CardNumber { get; set; } = string.Empty;
    public string ExpiryMonth { get; set; } = string.Empty;
    public string ExpiryYear { get; set; } = string.Empty;
    public string Cvv { get; set; } = string.Empty;
}

public class ValidateCardResponse
{
    public bool IsValid { get; set; }
    public string CardType { get; set; } = string.Empty;
    public string Last4Digits { get; set; } = string.Empty;
    public bool ExpiryValid { get; set; }
}
