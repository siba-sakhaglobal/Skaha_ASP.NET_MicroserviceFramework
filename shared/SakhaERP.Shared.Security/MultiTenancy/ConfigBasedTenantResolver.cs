namespace SakhaERP.Shared.Security.MultiTenancy;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Development-only resolver that reads tenant config from appsettings.json.
/// Use this when Tenant Management Service is not yet running.
///
/// Config format in appsettings.json:
/// {
///   "MultiTenancy": {
///     "DefaultTenantId": "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee",
///     "Tenants": {
///       "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee": {
///         "TenantName": "Dev Tenant",
///         "DbType": "Shared",
///         "ConnectionString": "Host=localhost;...",
///         "EnabledModules": ["inventory", "purchase", "sales"]
///       }
///     }
///   }
/// }
/// </summary>
public class ConfigBasedTenantResolver : ITenantConnectionResolver
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<ConfigBasedTenantResolver> _logger;

    public ConfigBasedTenantResolver(IConfiguration configuration, ILogger<ConfigBasedTenantResolver> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public Task<TenantInfo?> ResolveAsync(Guid tenantId, CancellationToken ct = default)
    {
        var tenantKey = tenantId.ToString();
        var section = _configuration.GetSection($"MultiTenancy:Tenants:{tenantKey}");

        if (!section.Exists())
        {
            // Fallback: use default connection string for any tenant (single shared DB for dev)
            var defaultConn = _configuration.GetConnectionString("InventoryDb")
                ?? "Host=localhost;Port=5432;Database=inventory_db;Username=erp_dev;Password=dev_password";

            _logger.LogDebug("Tenant {TenantId} not in config, using default connection string", tenantId);

            return Task.FromResult<TenantInfo?>(new TenantInfo
            {
                TenantId = tenantId,
                TenantName = "Default Dev Tenant",
                DbType = "Shared",
                ConnectionString = defaultConn,
                IsActive = true,
                EnabledModules = ["inventory", "purchase", "sales", "items", "settings"]
            });
        }

        var info = new TenantInfo
        {
            TenantId = tenantId,
            TenantName = section.GetValue<string>("TenantName") ?? "Unknown",
            DbType = section.GetValue<string>("DbType") ?? "Shared",
            ConnectionString = section.GetValue<string>("ConnectionString")
                ?? _configuration.GetConnectionString("InventoryDb") ?? "",
            EncryptionKeyId = section.GetValue<string>("EncryptionKeyId"),
            IsActive = section.GetValue("IsActive", true),
            EnabledModules = section.GetSection("EnabledModules").Get<HashSet<string>>() ?? []
        };

        _logger.LogDebug("Tenant {TenantId} resolved from config ({DbType} DB)", tenantId, info.DbType);
        return Task.FromResult<TenantInfo?>(info);
    }
}
