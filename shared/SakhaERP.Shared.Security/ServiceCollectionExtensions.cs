namespace SakhaERP.Shared.Security;

using SakhaERP.Shared.Security.MultiTenancy;
using Microsoft.Extensions.DependencyInjection;

public static class SecurityServiceCollectionExtensions
{
    /// <summary>
    /// Registers tenant resolution, ITenantContext, and multi-tenancy infrastructure.
    /// Use useConfigResolver=true for local dev (reads from appsettings.json).
    /// Use useConfigResolver=false for production (calls Tenant Service API + Redis cache).
    /// </summary>
    public static IServiceCollection AddSakhaSecurity(this IServiceCollection services, bool useConfigResolver = true)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<ITenantContext, TenantContext>();

        // Multi-tenancy: register tenant connection resolver
        if (useConfigResolver)
        {
            // Local dev: resolve from appsettings.json
            services.AddSingleton<ITenantConnectionResolver, ConfigBasedTenantResolver>();
        }
        else
        {
            // Production: resolve from Tenant Service API + Redis cache
            services.AddScoped<ITenantConnectionResolver, CachedTenantConnectionResolver>();
        }

        return services;
    }
}
