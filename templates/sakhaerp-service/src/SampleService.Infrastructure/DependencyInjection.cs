namespace SampleService.Infrastructure;

using SakhaERP.Shared.Security.MultiTenancy;
using SampleService.Application.Common.Interfaces;
using SampleService.Infrastructure.Persistence;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Register the default DbContext options (used by EF Core tools for migrations).
        services.AddDbContext<SampleServiceDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("SampleServiceDb"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(typeof(SampleServiceDbContext).Assembly.FullName)
            ), ServiceLifetime.Scoped);

        // Register ISampleServiceDbContext as a factory that resolves per-request tenant config.
        //   - Reads TenantInfo from HttpContext.Items (set by TenantMiddleware)
        //   - Shared DB: uses the shared connection string + sets tenant_id for query filters
        //   - Dedicated DB: uses the tenant-specific connection string
        services.AddScoped<ISampleServiceDbContext>(sp =>
        {
            var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
            var tenantInfo = httpContextAccessor.HttpContext?.Items[TenantMiddleware.TenantInfoKey] as TenantInfo;

            if (tenantInfo != null && !string.IsNullOrEmpty(tenantInfo.ConnectionString))
            {
                var optionsBuilder = new DbContextOptionsBuilder<SampleServiceDbContext>();
                optionsBuilder.UseNpgsql(tenantInfo.ConnectionString);

                return new SampleServiceDbContext(
                    optionsBuilder.Options,
                    tenantId: tenantInfo.TenantId,
                    isSharedDb: tenantInfo.IsSharedDb
                );
            }

            // Fallback: default registered DbContext (dev, migrations, background jobs).
            return sp.GetRequiredService<SampleServiceDbContext>();
        });

        var redisConnection = configuration.GetConnectionString("Redis");
        if (!string.IsNullOrEmpty(redisConnection))
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisConnection;
                options.InstanceName = "SampleService:";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}
