namespace SakhaERP.Shared.Security.MultiTenancy;

using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

/// <summary>
/// Production implementation: resolves tenant config with Redis cache + Tenant Service fallback.
///
/// For local development, use <see cref="ConfigBasedTenantResolver"/> which reads from appsettings.json.
/// </summary>
public class CachedTenantConnectionResolver : ITenantConnectionResolver
{
    private readonly IDistributedCache _cache;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<CachedTenantConnectionResolver> _logger;
    private readonly TimeSpan _cacheTtl;

    public CachedTenantConnectionResolver(
        IDistributedCache cache,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<CachedTenantConnectionResolver> logger)
    {
        _cache = cache;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
        _cacheTtl = TimeSpan.FromMinutes(configuration.GetValue("MultiTenancy:CacheTtlMinutes", 10));
    }

    public async Task<TenantInfo?> ResolveAsync(Guid tenantId, CancellationToken ct = default)
    {
        var cacheKey = $"tenant:{tenantId}:config";

        // 1. Try Redis cache
        var cached = await _cache.GetStringAsync(cacheKey, ct);
        if (!string.IsNullOrEmpty(cached))
        {
            _logger.LogDebug("Tenant {TenantId} config resolved from cache", tenantId);
            return JsonSerializer.Deserialize<TenantInfo>(cached);
        }

        // 2. Call Tenant Management Service
        try
        {
            var client = _httpClientFactory.CreateClient("tenant-service");
            var response = await client.GetAsync($"/api/v1/tenants/{tenantId}/config", ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning("Tenant {TenantId} not found or inactive. Status: {Status}",
                    tenantId, response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync(ct);
            var tenantInfo = JsonSerializer.Deserialize<TenantInfo>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (tenantInfo == null) return null;

            // 3. Cache the result
            await _cache.SetStringAsync(cacheKey, JsonSerializer.Serialize(tenantInfo),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = _cacheTtl }, ct);

            _logger.LogInformation("Tenant {TenantId} config resolved from Tenant Service and cached ({DbType} DB)",
                tenantId, tenantInfo.DbType);

            return tenantInfo;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to resolve tenant {TenantId} config from Tenant Service", tenantId);
            return null;
        }
    }
}
