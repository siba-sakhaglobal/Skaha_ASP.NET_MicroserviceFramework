namespace InventoryService.Infrastructure.Persistence;

using InventoryService.Application.Common.Interfaces;
using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Tenant-aware DbContext for the Inventory Service.
///
/// Multi-tenancy strategy (from architecture doc Section 7):
/// - Shared DB: Global query filter on tenant_id auto-applied to all queries.
///   PostgreSQL RLS provides defense-in-depth at the DB level.
/// - Dedicated DB: No tenant_id filter needed (whole DB belongs to one tenant).
///   Separate encryption key per tenant DB (via Azure Key Vault).
///
/// The correct connection string is resolved by ITenantConnectionResolver
/// and injected via DbContextOptions at request scope.
/// </summary>
public class InventoryDbContext : DbContext, IInventoryDbContext
{
    private readonly Guid _tenantId;
    private readonly bool _isSharedDb;

    /// <summary>
    /// Constructor for request-scoped context.
    /// tenantId and isSharedDb are set by the DI factory based on TenantInfo.
    /// </summary>
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options, Guid tenantId, bool isSharedDb)
        : base(options)
    {
        _tenantId = tenantId;
        _isSharedDb = isSharedDb;
    }

    /// <summary>
    /// Fallback constructor for EF Core design-time tools (migrations).
    /// </summary>
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        : base(options)
    {
        _tenantId = Guid.Empty;
        _isSharedDb = true;
    }

    // DbSets
    public DbSet<ItemWarehouseMapping> ItemWarehouseMappings => Set<ItemWarehouseMapping>();
    public DbSet<GateInward> GateInwards => Set<GateInward>();
    public DbSet<GateInwardItem> GateInwardItems => Set<GateInwardItem>();
    public DbSet<GoodsReceivedNote> GoodsReceivedNotes => Set<GoodsReceivedNote>();
    public DbSet<GrnLineItem> GrnLineItems => Set<GrnLineItem>();
    public DbSet<StockBalance> StockBalances => Set<StockBalance>();
    public DbSet<StockLedgerEntry> StockLedgerEntries => Set<StockLedgerEntry>();
    public DbSet<StockTransfer> StockTransfers => Set<StockTransfer>();
    public DbSet<StockTransferLine> StockTransferLines => Set<StockTransferLine>();
    public DbSet<StockAdjustment> StockAdjustments => Set<StockAdjustment>();
    public DbSet<StockAdjustmentLine> StockAdjustmentLines => Set<StockAdjustmentLine>();
    public DbSet<Batch> Batches => Set<Batch>();
    public DbSet<SerialNumber> SerialNumbers => Set<SerialNumber>();
    public DbSet<InventorySettings> InventorySettings => Set<InventorySettings>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(InventoryDbContext).Assembly);

        // SHARED DB: Apply global query filters for tenant isolation.
        // Every query automatically gets WHERE tenant_id = @tenantId AND is_deleted = false.
        // This is the EF Core layer of defense. PostgreSQL RLS is the DB layer of defense.
        //
        // DEDICATED DB: No tenant filter needed — entire DB belongs to one tenant.
        // We still apply soft-delete filter.
        if (_isSharedDb && _tenantId != Guid.Empty)
        {
            // AuditableEntity-based tables: filter by tenant + soft delete
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(
                        BuildTenantFilter(entityType.ClrType));
                }
            }

            // StockLedgerEntry is not AuditableEntity but has TenantId
            modelBuilder.Entity<StockLedgerEntry>()
                .HasQueryFilter(e => e.TenantId == _tenantId);
        }

        base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Builds a lambda expression: e => e.TenantId == _tenantId && !e.IsDeleted
    /// This is applied as a global query filter for shared DB tenancy.
    /// </summary>
    private System.Linq.Expressions.LambdaExpression BuildTenantFilter(Type entityType)
    {
        var param = System.Linq.Expressions.Expression.Parameter(entityType, "e");
        var tenantProp = System.Linq.Expressions.Expression.Property(param, nameof(AuditableEntity.TenantId));
        var tenantValue = System.Linq.Expressions.Expression.Constant(_tenantId);
        var tenantEquals = System.Linq.Expressions.Expression.Equal(tenantProp, tenantValue);

        var deletedProp = System.Linq.Expressions.Expression.Property(param, nameof(AuditableEntity.IsDeleted));
        var notDeleted = System.Linq.Expressions.Expression.Not(deletedProp);

        var combined = System.Linq.Expressions.Expression.AndAlso(tenantEquals, notDeleted);
        return System.Linq.Expressions.Expression.Lambda(combined, param);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
                    // Always stamp tenant_id on new entities (even in dedicated DB for consistency)
                    if (_tenantId != Guid.Empty)
                        entry.Entity.TenantId = _tenantId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTimeOffset.UtcNow;
                    // Prevent accidental tenant_id change
                    entry.Property(nameof(AuditableEntity.TenantId)).IsModified = false;
                    break;
            }
        }

        // StockLedgerEntry: stamp tenant on creation
        foreach (var entry in ChangeTracker.Entries<StockLedgerEntry>()
                     .Where(e => e.State == EntityState.Added))
        {
            if (_tenantId != Guid.Empty)
                entry.Entity.TenantId = _tenantId;
            entry.Entity.CreatedAt = DateTimeOffset.UtcNow;
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
