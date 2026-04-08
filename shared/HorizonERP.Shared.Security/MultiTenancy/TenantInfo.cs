namespace HorizonERP.Shared.Security.MultiTenancy;

/// <summary>
/// Resolved tenant configuration for the current request.
/// Populated by ITenantConnectionResolver, consumed by DbContext factory.
/// </summary>
public class TenantInfo
{
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;

    /// <summary>"Shared" or "Dedicated" — determines DB connection strategy.</summary>
    public string DbType { get; set; } = "Shared";

    /// <summary>
    /// For Dedicated: full connection string to tenant-specific DB.
    /// For Shared: connection string to shared DB (same for all shared tenants).
    /// </summary>
    public string ConnectionString { get; set; } = string.Empty;

    /// <summary>Azure Key Vault key identifier for tenant-specific encryption (dedicated DB).</summary>
    public string? EncryptionKeyId { get; set; }

    /// <summary>Modules enabled for this tenant (for feature gating).</summary>
    public HashSet<string> EnabledModules { get; set; } = [];

    /// <summary>Is the tenant active (subscription valid, not disabled)?</summary>
    public bool IsActive { get; set; } = true;

    public bool IsDedicatedDb => DbType.Equals("Dedicated", StringComparison.OrdinalIgnoreCase);
    public bool IsSharedDb => !IsDedicatedDb;
}
