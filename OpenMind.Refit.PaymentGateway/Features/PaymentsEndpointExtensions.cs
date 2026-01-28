using OpenMind.Refit.PaymentGateway.Features.Payments.GetPayment;
using OpenMind.Refit.PaymentGateway.Features.Payments.ProcessPayment;
using OpenMind.Refit.PaymentGateway.Features.Payments.RefundPayment;
using OpenMind.Refit.PaymentGateway.Features.Payments.ValidateCard;

namespace OpenMind.Refit.PaymentGateway.Features;

public static class PaymentsEndpointExtensions
{
    public static IEndpointRouteBuilder MapPaymentEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapProcessPaymentEndpoint();
        app.MapGetPaymentEndpoint();
        app.MapRefundPaymentEndpoint();
        app.MapValidateCardEndpoint();

        return app;
    }
}
