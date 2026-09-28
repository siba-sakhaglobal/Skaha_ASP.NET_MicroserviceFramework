# Architecture Docs — Read This First

Read [`framework-rules.md`](framework-rules.md) and [`tech-stack.md`](tech-stack.md) first — they
cover the boundaries and stack every other doc here assumes. After that, read whichever doc is
relevant to your task; you don't need all of them before you start.

| Doc | What it covers |
|---|---|
| [`framework-rules.md`](framework-rules.md) | The non-negotiable rules: mono-repo package boundaries, Clean Architecture per service, repository layout, CI gates. Start here. |
| [`tech-stack.md`](tech-stack.md) | Backend/frontend stack, key libraries, and coding standards. |
| [`naming-conventions.md`](naming-conventions.md) | Cloud resource naming, AKS namespaces, .NET/DTO naming, environment codes. |
| [`resilience.md`](resilience.md) | Saga/compensation patterns, circuit breakers, retries, idempotency, dead-letter queue handling. |
| [`security.md`](security.md) | Authn/authz, encryption at-rest/in-transit, Key Vault, network security, Zero Trust, privacy compliance. |
| [`multi-tenancy.md`](multi-tenancy.md) | The two supported tenancy models (shared DB + RLS vs. dedicated DB), how a tenant is resolved, and the decision matrix for migrating a tenant between them. |
| [`multi-tenancy-implementation.md`](multi-tenancy-implementation.md) | The actual middleware, EF Core, and RLS code that implements multi-tenancy — grounded in `services/inventory-service` as the reference implementation. |
| [`dto-conventions.md`](dto-conventions.md) | API response envelope, pagination, error format (ProblemDetails), header conventions, DTO naming and shape. |
| [`performance.md`](performance.md) | Caching, auto-scaling, database scalability, and latency-target guidance. |
