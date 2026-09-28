# Naming Conventions

## Azure Resource Naming Pattern

```
<resource>-<env>-<service>-<tenant>
```

| Segment | Values |
|---------|--------|
| `<env>` | `dev`, `qa`, `pp` (preprod), `prod` |
| `<tenant>` | `tn001`, `tnXYZ` (short form); use `shared` for shared infra |

The pattern is cloud-agnostic in spirit — swap the resource-type prefixes below for your
provider's equivalents (AWS, GCP, self-hosted) and keep the `{env}-{service}-{resource-type}`
ordering.

## Resource Naming Examples

| Resource Type | Convention | Example |
|--------------|-----------|---------|
| Resource Group | `rg-<env>-<service>-<tenant>` | `rg-prod-orders-tn001` |
| VNet | `vnet-<env>-<region>-<tenant>` | `vnet-prod-eus-tn001` |
| Subnet | `snet-<purpose>` | `snet-db`, `snet-app`, `snet-aks`, `snet-gateway` |
| Storage Account | `st<env><tenant><service>` (no hyphens, max 24 chars) | `stprodtn001orders` |
| SQL / Postgres Server | `sql-<env>-<tenant>` | `sql-prod-tn001` |
| Cosmos DB Account | `cos-<env>-<service>-<tenant>` | `cos-qa-inventory-tn002` |
| Key Vault | `kv-<env>-<tenant>` | `kv-prod-tn001` |
| AKS Cluster | `aks-<env>-<purpose>` | `aks-prod-core` |
| Function App | `func-<env>-<purpose>` | `func-pp-orchestrator` |
| App Service | `app-<env>-<service>` | `app-dev-auth` |
| APIM Instance | `apim-<env>-<project>` | `apim-prod-platform` |
| Container Registry | `acr<env><project>` | `acrprodplatform` |
| Redis Cache | `redis-<env>-<tenant>` | `redis-prod-tn001` |

Maintain a central naming reference (this file, or an equivalent) so resource names stay
predictable across services rather than drifting per-team.

## Network Layout

```
Hub VNet
  ├── VPN gateway / ExpressRoute ingress-egress
  └── Private DNS Zone

Spoke VNet (per environment or per large tenant)
  ├── snet-aks     ← AKS node pools
  ├── snet-app     ← App Service / Function Apps
  ├── snet-db      ← PostgreSQL + Cosmos DB (private endpoints)
  └── snet-gateway ← APIM / App Gateway
```

- Hub and spokes connected via **VNet Peering**.
- Production and non-prod spokes are **never peered** to each other.
- All DB and storage access goes through **Private Endpoints** only — no public access.

## AKS Namespace Naming

```
<service>-namespace
```

Examples: `orders-namespace`, `inventory-namespace`, `auth-namespace`, `orchestrator-namespace`
(cross-service coordination, if you run one).

## .NET Coding Conventions

| Element | Convention | Example |
|---------|-----------|---------|
| Classes | PascalCase | `OrderService`, `PaymentHandler` |
| Methods | PascalCase | `ProcessPayment()`, `GetTenantConfig()` |
| Properties | PascalCase | `TenantId`, `CreatedAt` |
| Interfaces | PascalCase with `I` prefix | `IOrderRepository`, `IPaymentGateway` |
| Local variables | camelCase | `orderId`, `tenantConfig` |
| Method parameters | camelCase | `tenantId`, `requestDto` |
| Private fields | `_camelCase` | `_logger`, `_dbContext` |
| Constants | PascalCase or UPPER_SNAKE (team choice) | `MaxRetryCount` |
| Namespaces | PascalCase, mirror folder structure | `OrderService.Application.Commands` |

## Environment Codes

| Code | Environment |
|------|------------|
| `dev` | Development |
| `qa` | Quality Assurance / Testing |
| `pp` | Pre-Production |
| `prod` | Production |

## Pipeline & Helm Chart Naming

- Pipeline file: `/services/<service-name>/infra/pipeline.yml`
- Helm chart: `/services/<service-name>/infra/helm/` (`Chart.yaml` per service)
- Container image tag: `<registry>/<service>:<version>-<commitSHA>`

## Resource Governance Baseline

| Control | Rule |
|---------|------|
| SKU tiers | Lower SKUs for dev/qa/preprod; production-grade SKUs reserved for prod |
| Encryption at-rest | Customer-managed keys (CMK) via Key Vault on all databases and storage |
| Diagnostic settings | Enforce log streaming to Log Analytics + Storage on every resource |
| Resource locks | `CanNotDelete` lock on critical production resources |
| Backup policy | DB snapshots and Blob versioning must be enabled |
