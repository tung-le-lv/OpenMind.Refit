using System.Net.Http.Headers;

namespace OpenMind.Refit.OrderService.ExternalApis;

/// <summary>
/// DelegatingHandler that adds headers to all Refit HTTP requests.
/// Register with .AddHttpMessageHandler<AuthHeaderHandler>() when configuring the Refit client.
/// </summary>
public class AuthHeaderHandler(IConfiguration configuration) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var apiKey = configuration["ExternalApi:ApiKey"];
        if (!string.IsNullOrEmpty(apiKey))
        {
            request.Headers.Add("X-Api-Key", apiKey);
        }

        request.Headers.UserAgent.Clear();
        request.Headers.UserAgent.Add(new ProductInfoHeaderValue("OpenMind.OrderService", "1.0"));
        request.Headers.Add("X-Correlation-Id", Guid.NewGuid().ToString());

        return await base.SendAsync(request, cancellationToken);
    }
}
