namespace SampleService.Infrastructure.Persistence;

using SampleService.Application.Common.Interfaces;
using SampleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Tenant-aware DbContext for this service.
///
/// Multi-tenancy strategy:
/// - Shared DB: Global query filter on tenant_id auto-applied to all queries.
///   PostgreSQL RLS provides defense-in-depth at the DB level.
/// - Dedicated DB: No tenant_id filter needed (whole DB belongs to one tenant).
///
/// The correct connection string is resolved by ITenantConnectionResolver
/// (SakhaERP.Shared.Security) and injected via DbContextOptions at request scope —
/// see DependencyInjection.cs in this project.
/// </summary>
public class SampleServiceDbContext : DbContext, ISampleServiceDbContext
{
    private readonly Guid _tenantId;
    private readonly bool _isSharedDb;

    /// <summary>Constructor for request-scoped context; tenantId/isSharedDb set by the DI factory.</summary>
    public SampleServiceDbContext(DbContextOptions<SampleServiceDbContext> options, Guid tenantId, bool isSharedDb)
        : base(options)
    {
        _tenantId = tenantId;
        _isSharedDb = isSharedDb;
    }

    /// <summary>Fallback constructor for EF Core design-time tools (migrations) and tests.</summary>
    public SampleServiceDbContext(DbContextOptions<SampleServiceDbContext> options)
        : base(options)
    {
        _tenantId = Guid.Empty;
        _isSharedDb = true;
    }

    public DbSet<Widget> Widgets => Set<Widget>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SampleServiceDbContext).Assembly);

        if (_isSharedDb && _tenantId != Guid.Empty)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(AuditableEntity).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.Entity(entityType.ClrType).HasQueryFilter(BuildTenantFilter(entityType.ClrType));
                }
            }
        }

        base.OnModelCreating(modelBuilder);
    }

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
                    if (_tenantId != Guid.Empty)
                        entry.Entity.TenantId = _tenantId;
                    break;
                case EntityState.Modified:
                    entry.Entity.ModifiedAt = DateTimeOffset.UtcNow;
                    break;
            }
        }

        return base.SaveChangesAsync(cancellationToken);
    }
}
