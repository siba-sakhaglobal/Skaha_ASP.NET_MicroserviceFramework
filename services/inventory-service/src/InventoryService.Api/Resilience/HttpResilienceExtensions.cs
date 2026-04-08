namespace InventoryService.Api.Resilience;

using Microsoft.Extensions.Http.Resilience;
using Polly;

public static class HttpResilienceExtensions
{
    /// <summary>
    /// Registers resilient HTTP clients for inter-service communication.
    /// Each external service gets its own named HttpClient with retry + circuit breaker.
    /// Pattern: inject IHttpClientFactory, call CreateClient("items-service")
    /// </summary>
    public static IServiceCollection AddResilientHttpClients(this IServiceCollection services, IConfiguration configuration)
    {
        // Items Service — validate item existence, fetch item details
        services.AddHttpClient("items-service", client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue("ServiceUrls:ItemsService", "http://localhost:5010")!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler();

        // Purchase Service — validate PO for GRN
        services.AddHttpClient("purchase-service", client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue("ServiceUrls:PurchaseService", "http://localhost:5020")!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler();

        // Settings Service — validate warehouse/bin existence
        services.AddHttpClient("settings-service", client =>
            {
                client.BaseAddress = new Uri(configuration.GetValue("ServiceUrls:SettingsService", "http://localhost:5030")!);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            })
            .AddStandardResilienceHandler();

        return services;
    }
}
