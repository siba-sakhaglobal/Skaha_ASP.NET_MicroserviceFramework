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
a handler runs.
