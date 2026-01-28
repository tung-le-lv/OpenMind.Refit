using OpenMind.Refit.OrderService.ExternalApis;
using Refit;

namespace OpenMind.Refit.OrderService.Infrastructure;

public static class RefitServiceExtensions
{
    public static IServiceCollection AddRefitClients(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<AuthHeaderHandler>();

        // AddRefitClient registers the Refit interface with HttpClientFactory
        // RefitSettings allows customizing serialization (System.Text.Json or Newtonsoft.Json)
        services
            .AddRefitClient<IExternalOrderApi>(new RefitSettings
            {
                ContentSerializer = new SystemTextJsonContentSerializer(new System.Text.Json.JsonSerializerOptions
                {
                    PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase,
                    WriteIndented = false,
                    DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
                })
            })
            .ConfigureHttpClient(client =>
            {
                var baseUrl = configuration["ExternalApi:BaseUrl"] ?? "https://api.example.com";
                client.BaseAddress = new Uri(baseUrl);
                client.Timeout = TimeSpan.FromSeconds(30);
            })
            // AddHttpMessageHandler chains DelegatingHandlers for cross-cutting concerns
            .AddHttpMessageHandler<AuthHeaderHandler>()
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}
