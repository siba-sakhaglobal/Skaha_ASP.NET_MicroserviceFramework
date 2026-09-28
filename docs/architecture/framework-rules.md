# Architecture Rules (Non-Negotiable)

These are the boundaries that make SakhaERP a *framework* rather than a pile of services that
happen to share a repo. They're enforced in code review, and CI will eventually lint for the
`<ProjectReference>` violation below.

## Mono-repo with NuGet package boundaries

- Shared libraries (`SakhaERP.Shared.*`) are published as versioned NuGet packages — to
  [NuGet.org](https://nuget.org), a private feed (Azure Artifacts, GitHub Packages, ...), or the
  local feed in `nuget.config` for development.
- **Never use `<ProjectReference>` between services, or from a service to a shared library.**
  Only `<PackageReference>` to a versioned package for anything that crosses a service or
  library boundary.
- `<ProjectReference>` is fine *within* a single service, e.g. `Api → Application → Domain →
  Infrastructure`.
- Each service is independently buildable, testable, and deployable — no service's build should
  ever require another service's source tree.
- No direct database access across service boundaries. Services communicate via Service Bus
  events or gateway-routed HTTP — never a connection string to another service's schema.
- No shared runtime state between services (no shared static caches, no shared in-memory
  singletons across process boundaries).

Why this matters more here than in a typical template: the whole pitch of a multi-service
framework is that teams can own services independently. `<ProjectReference>` across a boundary
quietly reintroduces a monolith's coupling — one team's uncommitted change in a shared folder
breaks another team's build. Package boundaries force that coupling to go through a version
number, which is the whole point.

## Service internal structure (Clean Architecture)

Every service under `services/{service-name}/src/` follows the same four layers:

- **`{Service}.Api/`** — Controllers, middleware, filters, DI wiring, `Program.cs`. Talks to
  `Application`, never to `Infrastructure` directly.
- **`{Service}.Application/`** — CQRS commands/queries/handlers (MediatR), DTOs, FluentValidation
  validators. Depends only on `Domain` — never on `Infrastructure` or `Api`.
- **`{Service}.Domain/`** — Entities, value objects, domain events, enums. No framework
  dependencies at all (no EF Core, no ASP.NET Core) — this layer should compile against nothing
  but the BCL and `SakhaERP.Shared.Domain`.
- **`{Service}.Infrastructure/`** — EF Core `DbContext`, repositories, EF migrations, external
  service clients. Implements interfaces defined in `Application`.

Tests mirror this under `services/{service-name}/tests/`: `{Service}.UnitTests` (fast, no I/O)
and `{Service}.IntegrationTests` (real database via Testcontainers or similar).

## Multi-tenancy

Tenant resolution and schema/connection routing live in `SakhaERP.Shared.Security` and run as
middleware before a request reaches a controller — a service's `Application`/`Domain` code should
never need to know how a tenant was resolved, only that `TenantContext` is populated by the time
a handler runs. See [`multi-tenancy.md`](multi-tenancy.md) for the two supported tenancy models
and [`multi-tenancy-implementation.md`](multi-tenancy-implementation.md) for the actual middleware
and EF Core wiring.

## Repository structure

```
/
  services/<service-name>/
    src/{Service}.Api|Application|Domain|Infrastructure/
    tests/{Service}.UnitTests|IntegrationTests/
    infra/helm/                # Kubernetes deployment for this service
  shared/SakhaERP.Shared.*/    # Versioned packages — see the rule above
  templates/sakhaerp-service/  # `dotnet new` template for scaffolding a new service
  frontend/sakha-erp-web/      # Next.js app consuming the services
  infra/global/                # Cross-cutting infra (gateway routes, etc.)
  docs/architecture/           # This file and its neighbors
```

## Correct vs. wrong `.csproj` references

```xml
<!-- CORRECT: PackageReference for anything crossing a service/library boundary -->
<ItemGroup>
  <PackageReference Include="SakhaERP.Shared.Security" />
</ItemGroup>

<!-- CORRECT: ProjectReference only within the same service -->
<ItemGroup>
  <ProjectReference Include="../OrdersService.Application/OrdersService.Application.csproj" />
</ItemGroup>

<!-- WRONG — NEVER DO THIS:
<ProjectReference Include="../../../shared/SakhaERP.Shared.Security/SakhaERP.Shared.Security.csproj" />
<ProjectReference Include="../../inventory-service/src/InventoryService.Domain/InventoryService.Domain.csproj" />
-->
```

This isn't hypothetical — `inventory-service` itself shipped with the first violation until it was
found and fixed. If you're reviewing a PR and see `<ProjectReference>` reaching outside the current
service's own `src/`/`tests/` tree, that's a blocker.

## CI and quality gates

The committed [`.github/workflows/build.yml`](../../.github/workflows/build.yml) packs the shared
libraries, restores/builds/tests `SakhaERP.sln`, and [`codeql.yml`](../../.github/workflows/codeql.yml)
runs static analysis — both against a single mono-repo-wide pipeline, because there's currently one
reference service. As more services are added, split to **one pipeline per service with path-based
triggers** (`paths: include: [services/{service-name}/**]`) so that changing one service never
rebuilds or redeploys another — that's the point of the package-boundary rule above; the pipeline
topology should match it.

Target gates for any service's pipeline, reference-service exemptions aside:

| Gate | Threshold |
|---|---|
| Code coverage | ≥ 80% line & branch on `Application`/`Domain` |
| Static analysis | No critical/high issues (CodeQL, or your SAST of choice) |
| Secret scanning | Zero secrets (gitleaks or equivalent) |
| Unit + integration tests | All passing |

## Inter-service communication

| Pattern | When | Never |
|---|---|---|
| Async events (Service Bus / equivalent) | Guaranteed delivery, cross-service side effects | — |
| Pub/sub fan-out (Event Grid / equivalent) | Lightweight notifications, multiple interested services | — |
| HTTP via the gateway | Synchronous read, rare, with a circuit breaker (`Microsoft.Extensions.Http.Resilience`) | Never direct service-to-service HTTP bypassing the gateway |
| Direct database query into another service's schema | — | **Never, full stop** |
| Shared in-memory state / static cache across services | — | **Never** — each service owns its own cache |
