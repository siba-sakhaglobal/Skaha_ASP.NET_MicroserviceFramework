# SakhaERP service template

Scaffolds a new service with the same shape as `services/inventory-service`: Clean
Architecture (`Api → Application → Domain → Infrastructure`), CQRS via MediatR, FluentValidation,
multi-tenancy wired through `SakhaERP.Shared.Security`, per-tenant rate limiting, structured
exception handling, and Serilog — all already connected, with one example feature slice
(`Widgets`) showing the pattern end to end.

## Install and use

```bash
# From the repo root, once:
dotnet new install ./templates/sakhaerp-service

# Then, for each new service:
dotnet new sakhaerp-service -n OrdersService -o services/orders-service
```

This produces `services/orders-service/src/OrdersService.{Api,Application,Domain,Infrastructure}`
and `services/orders-service/tests/OrdersService.{UnitTests,IntegrationTests}`, with every
`SampleService`/`Widget` reference replaced by your service name.

## After generating

1. Add the new service's projects to `SakhaERP.sln`:
   `dotnet sln SakhaERP.sln add services/orders-service/src/**/*.csproj services/orders-service/tests/**/*.csproj`
2. Rename `Widgets` (and the `Widget` entity) to your service's real domain concepts, or delete
   it and start fresh — it exists to show the pattern, not to be kept.
3. Add a connection string for the new service's database to `appsettings.json` and a matching
   entry to the root `docker-compose.yml` (see how `inventory-db` is wired for `InventoryService`) —
   give it its own port.
4. If your service needs the RLS session-variable interceptor or calls other services over HTTP
   with Polly resilience, copy `TenantConnectionInterceptor` and the `Resilience/` folder from
   `services/inventory-service` — they were left out of the template to avoid baking in
   inventory-specific service names.
5. `dotnet ef migrations add InitialCreate --project src/OrdersService.Infrastructure --startup-project src/OrdersService.Api`

## What's intentionally not included

Controllers here only cover `Create` + `GetById` for `Widgets`, to keep the template short — copy
the `List`/`Update`/`Delete` pattern from `inventory-service`'s `ItemWarehouseMappingController`
if your service needs full CRUD. `infra/helm/` isn't generated either; copy and adapt
`services/inventory-service/infra/helm/` once the service is ready to deploy.
