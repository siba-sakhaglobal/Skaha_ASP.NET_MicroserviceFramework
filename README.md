# SakhaERP

[![Build & Test](https://github.com/<github-org>/sakhaerp/actions/workflows/build.yml/badge.svg)](https://github.com/<github-org>/sakhaerp/actions/workflows/build.yml)
[![CodeQL](https://github.com/<github-org>/sakhaerp/actions/workflows/codeql.yml/badge.svg)](https://github.com/<github-org>/sakhaerp/actions/workflows/codeql.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

A .NET framework for building **multi-tenant, event-driven microservice** systems — Clean
Architecture and CQRS per service, versioned shared libraries instead of shared source, and
schema-per-tenant isolation baked in from the start.

> **Status:** pre-1.0. The framework conventions are stable enough to build on; the shared
> libraries and reference service are still filling out. See [open issues](../../issues) for
> the roadmap.

## Why SakhaERP

Most Clean Architecture / CQRS templates you'll find (this one included, itself inspired by
[jeangatto's excellent single-service reference](https://github.com/jeangatto/ASP.NET-Core-Clean-Architecture-CQRS-Event-Sourcing))
show you how to structure *one* well-built service. SakhaERP is about what happens once you
need a *fleet* of them, owned by different teams, deployed independently, sharing conventions
but not source code:

- **Package boundaries, not project references.** Shared code (`SakhaERP.Shared.*`) ships as
  versioned NuGet packages. A service can upgrade a shared library on its own schedule — nobody's
  build breaks because another team pushed to a shared folder.
- **Schema-per-tenant multi-tenancy is a framework concern, not a per-service afterthought.**
  `SakhaERP.Shared.Security` resolves the tenant and sets the right DB schema before your
  handler ever runs.
- **Services talk over events, not databases.** No service reaches into another service's schema.
  Cross-service communication is Service Bus events or gateway-routed HTTP.
- **Every service looks the same.** `Api → Application → Domain → Infrastructure`, CQRS via
  MediatR, FluentValidation on every command, one middleware pipeline shape. A developer who
  knows one SakhaERP service can read any other one.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│  API Gateway (rate limiting, auth pre-check, TLS)            │
└───────────────┬─────────────────────────────┬─────────────────┘
                 │                             │
        ┌────────▼────────┐          ┌─────────▼────────┐
        │  Service A       │          │  Service B        │
        │  Api             │          │  Api               │
        │  Application     │  events  │  Application       │
        │  Domain          │◄────────►│  Domain             │
        │  Infrastructure  │  (bus)   │  Infrastructure     │
        └────────┬─────────┘          └─────────┬───────────┘
                 │                               │
     ┌───────────▼───────────┐       ┌───────────▼───────────┐
     │ tenant_{slug} schema   │       │ tenant_{slug} schema   │
     └────────────────────────┘       └────────────────────────┘

        Both services reference SakhaERP.Shared.* as NuGet packages,
        never as project references.
```

Each service follows the same internal layout:

```
services/{service-name}/
  src/
    {Service}.Api/             # Controllers, middleware, DI wiring
    {Service}.Application/     # MediatR commands/queries/handlers, validators, DTOs
    {Service}.Domain/          # Entities, value objects, domain events — no framework deps
    {Service}.Infrastructure/  # EF Core DbContext, repositories, migrations
  tests/
    {Service}.UnitTests/
    {Service}.IntegrationTests/
  infra/helm/                  # Kubernetes deployment
```

Full rules (package boundaries, no cross-service DB access, layer responsibilities) are in
[`docs/architecture/framework-rules.md`](docs/architecture/framework-rules.md).

## Tech stack

.NET 9 · ASP.NET Core · Entity Framework Core 9 · PostgreSQL 16 (schema-per-tenant) · Redis ·
MediatR (CQRS) · FluentValidation · Polly (resilience) · Serilog · xUnit + FluentAssertions

## Getting started

```bash
git clone <this-repo>
cd erp-mono-repo
./scripts/bootstrap.sh   # packs shared libs into the local NuGet feed, restores SakhaERP.sln
docker compose up -d     # Postgres + Redis for the reference service
dotnet run --project services/inventory-service/src/InventoryService.Api
```

The API comes up with Swagger at `/swagger`. `services/inventory-service` is the reference
implementation — read it to see the framework's conventions applied, not as a spec for an
inventory system.

### Running tests

```bash
dotnet test SakhaERP.sln
```

## Scaffolding a new service

```bash
dotnet new install ./templates/sakhaerp-service   # once per clone
dotnet new sakhaerp-service -n OrdersService -o services/orders-service
dotnet sln SakhaERP.sln add services/orders-service/src/**/*.csproj services/orders-service/tests/**/*.csproj
```

This generates a service with the same Clean Architecture layout, multi-tenancy wiring, and CQRS
pipeline as `inventory-service`, with one example feature slice to delete or rename. See
[`templates/sakhaerp-service/README.md`](templates/sakhaerp-service/README.md) for what to do next.

## Repository layout

```
shared/     SakhaERP.Shared.* — versioned packages consumed by every service
services/   One folder per microservice, each independently buildable/deployable
templates/  `dotnet new` template for scaffolding a new service
packages/   Local NuGet feed (git-ignored) used by scripts/bootstrap.sh until Azure Artifacts is wired up
infra/      Cross-cutting infrastructure (gateway routes, etc.)
docs/       Architecture docs and API specs
```

## Contributing

See [`CONTRIBUTING.md`](CONTRIBUTING.md). The architectural rules there are enforced in review —
please read them before opening a PR that adds a service or touches `shared/`.

## License

[MIT](LICENSE) © Sakha Global
