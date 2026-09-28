# Multi-Tenancy Implementation Guide

How the hybrid multi-tenancy model ([`multi-tenancy.md`](multi-tenancy.md)) is implemented in
code. `services/inventory-service` is the reference implementation — the same wiring pattern
applies to any service you scaffold from `templates/sakhaerp-service`.

## Three Layers of Tenant Isolation

| Layer | Mechanism | Shared DB | Dedicated DB |
|-------|-----------|-----------|-------------|
| **1. Middleware** | `TenantMiddleware` | Resolves `tenant_id` → `TenantInfo` | Same |
| **2. EF Core** | Global query filter | `WHERE tenant_id = @id AND !is_deleted` | No automatic filter — the whole DB belongs to one tenant |
| **3. PostgreSQL** | Row-Level Security (RLS) | `SET app.current_tenant_id; RLS policy` | Not needed |

## Request Flow

```
HTTP Request (JWT has tenant_id)
  │
  ▼
TenantMiddleware
  ├─ Extract tenant_id from JWT claim ("tenant_id") or X-Tenant-Id header
  ├─ Call ITenantConnectionResolver.ResolveAsync(tenantId)
  │    ├─ Check Redis cache ("tenant:{id}:config")
  │    └─ Cache miss → call the Tenant Management Service API
  ├─ Reject 403 if tenant not found or inactive
  └─ Store TenantInfo in HttpContext.Items["CurrentTenant"]
  │
  ▼
DI factory creates the service's DbContext (e.g. InventoryDbContext)
  ├─ Read TenantInfo from HttpContext.Items
  ├─ If Shared DB:
  │    ├─ Use the shared connection string
  │    ├─ Pass tenantId + isSharedDb into the DbContext for EF Core query filters
  │    └─ Add TenantConnectionInterceptor (sets the PostgreSQL session var for RLS)
  └─ If Dedicated DB:
       ├─ Use the tenant-specific connection string
       └─ No tenant filter needed (entire DB is theirs)
  │
  ▼
EF Core Query Execution
  ├─ Shared DB: every query on an AuditableEntity gets WHERE tenant_id = @id AND is_deleted = false
  ├─ Dedicated DB: no automatic filter applied — the DB boundary is the isolation
  └─ SaveChangesAsync auto-stamps tenant_id on new entities, and blocks tenant_id changes on updates
  │
  ▼
PostgreSQL RLS (Shared DB only)
  └─ Even raw SQL or a bypassed EF Core filter can't see another tenant's rows
```

`Application`/`Domain` code never resolves any of this itself — it injects `ITenantContext`
(from `SakhaERP.Shared.Security`) to read the current tenant/user, and everything above already
ran by the time a handler executes:

```csharp
public interface ITenantContext
{
    Guid TenantId { get; }
    Guid UserId { get; }
    string? UserRole { get; }
}
```

## Key Files

| File | Location | Purpose |
|------|----------|---------|
| `TenantInfo.cs` | `shared/SakhaERP.Shared.Security/MultiTenancy/` | Resolved tenant config DTO (connection string, `DbType`, enabled modules) |
| `ITenantConnectionResolver.cs` | Same | Interface for resolving tenant config |
| `ConfigBasedTenantResolver.cs` | Same | Dev: reads from `appsettings.json`, falls back to a default connection string if the tenant isn't configured |
| `CachedTenantConnectionResolver.cs` | Same | Prod: Redis cache + Tenant Service API |
| `TenantMiddleware.cs` | Same | Resolves the tenant per request, rejects invalid/inactive tenants |
| `TenantContext.cs` | Same | `ITenantContext` — what `Application`/`Domain` code injects to read the current tenant/user |
| `SecurityServiceCollectionExtensions.cs` (`AddSakhaSecurity`) | Same | DI registration: wires up `ITenantContext` and picks the resolver (`useConfigResolver: true` for dev, `false` for prod) |
| `InventoryDbContext.cs` | `services/inventory-service/src/InventoryService.Infrastructure/Persistence/` | Tenant-aware `DbContext` with the dynamic query filters described above |
| `TenantConnectionInterceptor.cs` | Same | Sets the PostgreSQL session variable for RLS |
| `RlsSetup.sql` | Same | SQL that enables RLS policies on all tenant-scoped tables |
| `DependencyInjection.cs` | `services/inventory-service/src/InventoryService.Infrastructure/` | `AddInfrastructure()` — factory that builds the `DbContext` with the correct connection string per request |

## Wiring It Up in a Service

`Program.cs` registers multi-tenancy and puts `TenantMiddleware` in the pipeline before anything
that needs `TenantInfo` (rate limiting, in this example):

```csharp
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSakhaSecurity(useConfigResolver: builder.Environment.IsDevelopment());
// ...
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseMiddleware<TenantMiddleware>();   // resolves tenant_id → connection string, shared/dedicated DB
app.UseRateLimiter();                    // per-tenant, needs tenant resolved first
```

`AddInfrastructure` registers `IInventoryDbContext` as a factory (not a plain `AddDbContext`)
specifically so it can read `TenantInfo` from `HttpContext.Items` at resolution time and build a
`DbContextOptions` pointed at the right connection string:

```csharp
services.AddScoped<IInventoryDbContext>(sp =>
{
    var tenantInfo = sp.GetRequiredService<IHttpContextAccessor>()
        .HttpContext?.Items[TenantMiddleware.TenantInfoKey] as TenantInfo;

    if (tenantInfo != null && !string.IsNullOrEmpty(tenantInfo.ConnectionString))
    {
        var optionsBuilder = new DbContextOptionsBuilder<InventoryDbContext>();
        optionsBuilder.UseNpgsql(tenantInfo.ConnectionString);

        if (tenantInfo.IsSharedDb)
        {
            optionsBuilder.AddInterceptors(
                new TenantConnectionInterceptor(tenantInfo.TenantId, isSharedDb: true));
        }

        return new InventoryDbContext(optionsBuilder.Options, tenantInfo.TenantId, tenantInfo.IsSharedDb);
    }

    // Fallback: default DbContext, used for dev / EF migrations / background jobs
    return sp.GetRequiredService<InventoryDbContext>();
});
```

## Configuration (appsettings.json)

### Local Development (`ConfigBasedTenantResolver`)
```json
{
  "MultiTenancy": {
    "Tenants": {
      "aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee": {
        "TenantName": "Dev Tenant",
        "DbType": "Shared",
        "ConnectionString": "Host=localhost;...",
        "EnabledModules": ["inventory", "purchase"]
      }
    }
  }
}
```

If a tenant isn't listed under `MultiTenancy:Tenants`, `ConfigBasedTenantResolver` falls back to
the service's default `ConnectionStrings` entry — which is what the reference `inventory-service`
does out of the box, so local dev works with a single shared database without any per-tenant
config at all. Add entries here once you actually need to exercise dedicated-DB or multi-tenant
behavior locally.

### Production (`CachedTenantConnectionResolver`)
```json
{
  "MultiTenancy": {
    "CacheTtlMinutes": 10
  },
  "ServiceUrls": {
    "TenantService": "http://tenant-service.tenant-ns.svc.cluster.local"
  }
}
```

## Decision Matrix: Shared vs Dedicated

| Condition | Action |
|-----------|--------|
| DB size < 5 GB AND daily txns < 50K | Shared DB (`TenantId` column + RLS) |
| DB size ≥ 5 GB OR daily txns ≥ 50K | Migrate to dedicated DB |
| High-tier/enterprise tenant | Dedicated DB from day one, if that's your policy |

Migration from shared → dedicated is handled by the Tenant Management Service (or whatever owns
tenant lifecycle in your deployment) — not by individual services.

## Security Rules

- `SaveChangesAsync` prevents modification of `tenant_id` on existing entities
- `TenantMiddleware` rejects requests without a valid `tenant_id` (403)
- `TenantMiddleware` rejects inactive tenants (403)
- PostgreSQL RLS uses `FORCE ROW LEVEL SECURITY` — even table owners can't bypass it
- The application DB user (e.g. `erp_app`) must **not** be a superuser; only the migration user
  needs elevated rights
- Encryption key per tenant stored in Key Vault (dedicated DB)
