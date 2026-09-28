namespace InventoryService.Infrastructure;

using SakhaERP.Shared.Security.MultiTenancy;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the default DbContext options (used by EF Core tools for migrations)
        services.AddDbContext<InventoryDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("InventoryDb"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(InventoryDbContext).Assembly.FullName)
            ), ServiceLifetime.Scoped);

        // Register IInventoryDbContext as a factory that resolves per-request tenant config.
        // This is the HYBRID multi-tenancy magic:
        //   - Reads TenantInfo from HttpContext.Items (set by TenantMiddleware)
        //   - For Shared DB: uses shared connection string + sets tenant_id for query filters
        //   - For Dedicated DB: uses tenant-specific connection string
        services.AddScoped<IInventoryDbContext>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var tenantInfo = httpContextAccessor.HttpContext?.Items[TenantMiddleware.TenantInfoKey] as TenantInfo;

            if (tenantInfo != null && !string.IsNullOrEmpty(tenantInfo.ConnectionString))
            {
                // Build DbContextOptions with tenant-specific connection string
                var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
                optionsBuilder.UseNpgsql(tenantInfo.ConnectionString);

                // For shared DB: add RLS interceptor that sets PostgreSQL session variable
                if (tenantInfo.IsSharedDb)
                {
                    optionsBuilder.AddInterceptors(
                        new Persistence.TenantConnectionInterceptor(tenantInfo.TenantId, isSharedDb: true));
                }

                return new InventoryDbContext(
                    optionsBuilder.Options,
                    tenantId: tenantInfo.TenantId,
                    isSharedDb: tenantInfo.IsSharedDb
                );
            }

            // Fallback: use the default registered DbContext (for dev, migrations, background jobs)
            var defaultContext = sp.GetRequiredService<InventoryDbContext>();
            return defaultContext;
        });

        // Redis Distributed Cache
        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "InventoryService:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
