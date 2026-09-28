# Multi-Tenancy

## Hybrid Tenancy Model

| Tenant Type | Database Strategy | Isolation Mechanism |
|-------------|------------------|-------------------|
| Small tenants (low volume) | Shared DB with `TenantId` column | Row-Level Security (RLS) |
| Large tenants (high volume) | Dedicated DB per tenant | Separate connection string per tenant |

Both models coexist — a given deployment can run some tenants on the shared DB and others on
dedicated DBs at the same time. See
[`multi-tenancy-implementation.md`](multi-tenancy-implementation.md) for how a service picks which
one applies to the current request.

## Decision Matrix: When to Migrate to Dedicated DB

Migrate a tenant from shared to dedicated DB when **either** condition is met:

| Threshold | Value |
|-----------|-------|
| DB size exceeds | 5 GB |
| Daily transaction count exceeds | 50,000 |

Track these thresholds per tenant and trigger a migration (automated or manual, depending on how
much you want to invest in that tooling) when either is breached.

## How a Tenant Is Resolved

1. The tenant ID comes from a JWT claim (or an `X-Tenant-Id` header for server-to-server calls) —
   never from unvalidated request input.
2. A resolver looks up that tenant's config (connection string, DB type, enabled modules, active
   status) — typically cached, with a fallback to a tenant management service on a cache miss.
3. Requests for an unknown or inactive tenant are rejected before they reach any business logic.
4. The resolved config drives which connection string and isolation mechanism (RLS vs. dedicated
   DB) the rest of the request uses.

The concrete middleware and EF Core wiring for this are in
[`multi-tenancy-implementation.md`](multi-tenancy-implementation.md).

## Tenant Configuration

A resolved tenant's config is the shape downstream code needs to route a request correctly —
nothing product-specific belongs in it:

```json
{
  "tenantId": "tn001",
  "dbType": "Shared",
  "connectionString": "<connection-string-or-shared-key>",
  "enabledModules": ["orders", "inventory"],
  "authSettings": { "provider": "EntraID", "mfaRequired": true, "ssoEnabled": true },
  "isActive": true,
  "dataRegion": "eastus"
}
```

Keep this config free of anything that belongs to a specific product's business workflow (billing
plans, onboarding state, UI theme) — those live in whatever service owns that concern, not in the
tenant resolution config every service depends on.

## Data Isolation & Compliance

- **Shared DB**: row-level security enforced at the PostgreSQL level; `TenantId` column on all tables
- **Dedicated DB**: separate encryption key per tenant DB
- **Privacy compliance**: masking and consent logging as needed for your jurisdiction; a
  Right-to-be-Forgotten workflow should be automatable (see [`security.md`](security.md))
- **Data residency**: DB deployed in the region required for that tenant, if that matters for your
  deployment
- **JWT tokens**: always include `TenantId` + `Role` claims; RBAC enforced at Tenant → Module →
  Feature → Action level

## Monitoring & Scaling Thresholds

- Track per-tenant: DB size, daily transaction count, active users, resource usage
- Alert on: DB size approaching the dedicated-DB threshold, transaction volume approaching SLA limits
- Auto-scale AKS pods for heavy tenants (HPA based on CPU/memory/custom metrics)
- Per-tenant queue/topic segregation in Service Bus / Event Grid, if you use per-tenant queues
