namespace SakhaERP.Shared.Security.MultiTenancy;

/// <summary>
/// Resolves tenant configuration (connection string, DB type, encryption key) for the current request.
///
/// Resolution strategy:
/// 1. Check Redis cache (key: "tenant:{tenantId}:config")
/// 2. On cache miss: call Tenant Management Service API
/// 3. Cache result with 10-minute TTL (configurable)
///
/// Each microservice registers this in DI. The implementation uses IDistributedCache + IHttpClientFactory.
/// </summary>
public interface ITenantConnectionResolver
{
    /// <summary>Resolve tenant config for the given tenant ID.</summary>
    Task<TenantInfo?> ResolveAsync(Guid tenantId, CancellationToken ct = default);
}
