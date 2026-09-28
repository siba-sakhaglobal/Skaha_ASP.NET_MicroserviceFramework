# Performance & Scalability

## Latency Targets

| Operation | Target |
|-----------|--------|
| API responses (transactional) | ≤ 300 ms |
| Tenant config fetch | ≤ 300 ms |
| Language switch | ≤ 150 ms |
| Unit test execution in CI/CD | ≤ 5 minutes |

These are starting points, not hard framework requirements — set targets that match your own
SLAs and add rows for whatever operations matter to your product (e.g. anything calling an
external AI/LLM provider will have its own, much looser, latency budget).

## Caching (Redis per Tenant)

- **Service**: Azure Redis Cache (or your provider's managed Redis)
- Per-tenant cache keys enforced to maintain tenant isolation
- **What is cached**:
  - Tenant configuration and feature flags
  - Session data
  - Hot reference data (lookup tables, master data)
- Cache TTL set per data type — short for anything that changes often, longer for static
  reference data
- Config refresh: on an interval, or event-driven via SignalR/WebSocket
- Redis persistence enabled (RDB snapshot) for cache recovery

## CDN for Static Assets

- **Service**: Azure CDN (or your provider's CDN)
- Delivers React/Next.js static assets (JS, CSS, images) globally with low latency
- Caches: multilingual UI files, tenant-specific theme assets
- Origin: Blob Storage (multi-tenant asset hosting)
- Cache invalidation configured for dynamic module updates

## Auto-Scaling (HPA for AKS)

- **Horizontal Pod Autoscaler (HPA)** for all AKS services
- Scaling triggers: CPU utilization, memory usage, custom metrics (queue depth, request rate)
- App Service Plan scales out per module when not on AKS
- Event-driven scaling: AKS KEDA (Kubernetes Event-Driven Autoscaling) for queue-based workloads

## Database Scalability

| Technique | When Used |
|-----------|-----------|
| Horizontal partitioning (sharding) | Large tenant datasets in a dedicated DB |
| Vertical scaling (CPU, memory) | High-throughput transactional workloads |
| Read replicas | Read-heavy reporting queries |
| Shared DB with RLS | Small tenants (< 5 GB, < 50K daily transactions) |
| Dedicated DB | Large tenants (> 5 GB or > 50K daily transactions) |
| Change Data Capture (CDC) | Async sync from the primary DB to an analytics store |

## Event-Driven Architecture

- **Azure Service Bus**: guaranteed delivery for critical workflows
- **Azure Event Grid**: lightweight broadcast for notifications
- Benefits:
  - Decouples services from each other and from downstream consumers
  - Supports async workflows across services (e.g. order processing, notification fan-out)
  - Per-tenant queue/topic segregation to maintain isolation

## Load Balancing & Global Delivery

| Service | Role |
|---------|------|
| Azure Front Door | Multi-region load balancing, WAF, SSL termination, DNS failover, blue/green traffic switching |
| Application Gateway | Intra-region load balancing, SSL offload, path-based routing for API versioning |
| Azure APIM | API-level throttling, rate limiting per tenant, request routing |

- Front Door WAF protects all public endpoints
- Path-based routing supports API versioning and module routing

## Frontend Bundle Optimization

- **Code splitting**: each route/module loaded on-demand via `next/dynamic`
- **Tree shaking**: unused code eliminated from bundles
- **Lazy loading**: modules loaded only when the user navigates to them
- **Performance testing**: Lighthouse / Web Vitals (load time, rendering, bundle size)
- **State management**: Redux Toolkit / React Query for efficient re-rendering
- **BroadcastChannel**: multi-tab state sync without server round-trips

## .NET Performance Patterns

- Use `.AsNoTracking()` for all read-only EF Core queries
- Use `.Any()` instead of `.Count() > 0`
- Avoid `.ToList()` unless materialization is required
- Paginate all queries; avoid unbounded result sets
- Use `Span<T>` / `Memory<T>` for high-volume data processing
- Always `Dispose` / `await using` on `DbContext` and `HttpClient`
- `ConfigureAwait(false)` in library code
- Never block async with `.Result` or `.Wait()`

## Monitoring for Performance

- **Azure Monitor**: pod/instance CPU, memory utilization
- **Application Insights**: request latency, dependency timing, exception rates per service
- **Log Analytics**: centralized query across all services
- **Grafana dashboards**: real-time visual monitoring
- Track: queue depth, processing latency, tenant-specific load
- Alerts configured for: SLA threshold breaches, latency spikes, high error rates
