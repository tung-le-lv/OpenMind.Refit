using System.Net.Http.Headers;

namespace OpenMind.Refit.OrderService.ExternalApis;

/// <summary>
/// DelegatingHandler that adds headers to all Refit HTTP requests.
/// Register with .AddHttpMessageHandler&lt;AuthHeaderHandler&gt;() when configuring the Refit client.
/// This handler adds correlation ID and user agent for tracing service-to-service calls.
/// </summary>
public class AuthHeaderHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        request.Headers.UserAgent.Clear();
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OpenMind.OrderService", "1.0"));
        
        // X-Correlation-Id helps trace requests across multiple services
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());

        return await base.SendAsync(request, cancellationToken);
    }
}
