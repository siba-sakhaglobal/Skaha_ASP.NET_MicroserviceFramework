namespace InventoryService.Api.RateLimiting;

using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;

public static class TenantRateLimitingExtensions
{
    public const string TenantPolicy = "per-tenant";

    public static IServiceCollection AddTenantRateLimiting(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddRateLimiter(options =>
        {
            // Per-tenant sliding window: prevents noisy-neighbor problem
            options.AddPolicy(TenantPolicy, context =>
            {
                // Extract tenant from JWT claim or header
                var tenantId = context.User?.FindFirst("tenant_id")?.Value
                    ?? context.Request.Headers["X-Tenant-Id"].FirstOrDefault()
                    ?? "anonymous";

                return RateLimitPartition.GetSlidingWindowLimiter(tenantId, _ => new SlidingWindowRateLimiterOptions
                {
                    PermitLimit = configuration.GetValue("RateLimiting:PermitLimit", 100),
                    Window = TimeSpan.FromSeconds(configuration.GetValue("RateLimiting:WindowSeconds", 60)),
                    SegmentsPerWindow = 4,
                    QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                    QueueLimit = configuration.GetValue("RateLimiting:QueueLimit", 10)
                });
            });

            // Global fallback for unauthenticated requests
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            options.OnRejected = async (context, ct) =>
            {
                context.HttpContext.Response.ContentType = "application/json";
                await context.HttpContext.Response.WriteAsJsonAsync(new
                {
                    success = false,
                    errors = new[]
                    {
                        new { type = "RateLimitExceeded", title = "Too Many Requests", status = 429,
                              detail = "Rate limit exceeded for this tenant. Please retry after a moment." }
                    }
                }, ct);
            };
        });

        return services;
    }
}
