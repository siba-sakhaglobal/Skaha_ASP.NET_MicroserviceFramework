namespace InventoryService.Infrastructure.Persistence;

using System.Data.Common;
using Microsoft.EntityFrameworkCore.Diagnostics;

/// <summary>
/// EF Core connection interceptor that sets the PostgreSQL session variable
/// for Row-Level Security (RLS) on shared databases.
///
/// When a connection opens, this interceptor executes:
///   SET app.current_tenant_id = '{tenantId}';
///
/// PostgreSQL RLS policies then enforce: tenant can only see rows
/// WHERE tenant_id = current_setting('app.current_tenant_id')::uuid
///
/// This is the DB-level defense layer. EF Core global query filters
/// are the application-level layer. Together they provide defense-in-depth.
/// </summary>
public class TenantConnectionInterceptor : DbConnectionInterceptor
{
    private readonly Guid _tenantId;
    private readonly bool _isSharedDb;

    public TenantConnectionInterceptor(Guid tenantId, bool isSharedDb)
    {
        _tenantId = tenantId;
        _isSharedDb = isSharedDb;
    }

    public override async Task ConnectionOpenedAsync(
        DbConnection connection, ConnectionEndEventData eventData, CancellationToken ct = default)
    {
        if (_isSharedDb && _tenantId != Guid.Empty)
        {
            // Set PostgreSQL session variable for RLS
            await using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET app.current_tenant_id = '{_tenantId}';";
            await cmd.ExecuteNonQueryAsync(ct);
        }

        await base.ConnectionOpenedAsync(connection, eventData, ct);
    }

    public override void ConnectionOpened(
        DbConnection connection, ConnectionEndEventData eventData)
    {
        if (_isSharedDb && _tenantId != Guid.Empty)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = $"SET app.current_tenant_id = '{_tenantId}';";
            cmd.ExecuteNonQuery();
        }

        base.ConnectionOpened(connection, eventData);
    }
}
