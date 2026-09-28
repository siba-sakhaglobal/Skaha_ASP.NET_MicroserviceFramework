# Tech Stack

## Core platform

| Layer | Technology | Notes |
|---|---|---|
| Backend runtime | .NET 9 (C#) | ASP.NET Core Web API per service |
| Frontend | Next.js (React + TypeScript) | SSR + CSR, one app consuming the services |
| Database | PostgreSQL | One logical database per service; schema-per-tenant for shared-DB tenants |
| Containers | Docker | `docker-compose.yml` for local dev; each service ships its own Dockerfile |
| Cloud (reference deployment) | Azure (AKS, Service Bus, API Management, Key Vault) | The pattern (events over sync calls, gateway in front of every service, secrets out of code) is cloud-agnostic — swap in the AWS/GCP/self-hosted equivalents and the framework rules still hold |

## Backend libraries

| Library | Purpose |
|---|---|
| Serilog | Structured logging |
| Polly (`Microsoft.Extensions.Http.Resilience`) | Retry + circuit breaker for outbound HTTP |
| Entity Framework Core | Code-first ORM, migrations per service |
| MediatR | In-process CQRS dispatch (commands/queries → handlers) |
| FluentValidation | Command/query input validation, wired as a MediatR pipeline behavior |
| xUnit + FluentAssertions | Unit and integration tests |
| Moq | Test mocking |
| coverlet | Line/branch coverage |

## Frontend libraries

| Library | Purpose |
|---|---|
| React + TypeScript | Strict typing, no implicit `any` |
| Jest + React Testing Library | Unit testing |
| Tailwind-style utility CSS (see `frontend/sakha-erp-web/src/styles`) | Styling |

## .NET coding standards

- `PascalCase` for classes, methods, properties, interfaces; `camelCase` for locals/parameters; `_camelCase` for private fields.
- All I/O is `async`/`await` — never block with `.Result` or `.Wait()`; never `async void` outside event handlers.
- Constructor dependency injection; no `new`-ing services inside business logic.
- `<Nullable>enable</Nullable>` everywhere (set once, in `Directory.Build.props`).
- Parameterized queries only — EF Core LINQ, never raw SQL string concatenation.
- `.AsNoTracking()` on read-only EF Core queries; `.Any()` instead of `.Count() > 0`.

## Frontend standards

- TypeScript everywhere, no implicit `any`.
- Component-based, reusable hooks over prop-drilling.
- Tenant-aware theming and locale/currency formatting where the product needs it — not assumed by the framework itself.

## Service folder layout

See [`framework-rules.md`](framework-rules.md) for the full mono-repo and per-service layout, and [`../../CONTRIBUTING.md`](../../CONTRIBUTING.md) for how to scaffold a new one.
