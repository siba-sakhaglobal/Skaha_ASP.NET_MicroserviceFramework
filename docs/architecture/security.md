# Security

## Authentication

- **Service**: Azure Entra ID (Azure Active Directory), or any OIDC-compliant identity provider
- **Protocol**: OAuth2 / OpenID Connect
- **SSO**: Single sign-on across all your services
- **MFA**: Required for high-privilege users
- **Social logins**: Supported for external users via OAuth2/OpenID Connect
- **Fallback**: Form authentication when SSO is unavailable
- **JWT tokens**: Short-lived access tokens; include `TenantId` + `Role` claims
- **Refresh tokens**: Stored securely (HttpOnly cookies)
- Every service validates JWT tokens issued by the identity provider — never trust a token that
  was only validated at the gateway.

## Authorization

| Model | Detail |
|-------|--------|
| RBAC | Roles: Admin, Manager, Employee (or whatever your domain needs) — controls module and API access |
| ABAC | Fine-grained based on attributes: tenant, department, feature flags, time-of-day |
| Hierarchy | Tenant → Module → Feature → Action |
| Enforcement | Middleware in each backend service; `[Authorize]` attribute on all controllers |
| Gateway-level | APIM (or your gateway) policies enforce tenant-level authorization before requests reach services |

## Data Encryption

### At-Rest
- **PostgreSQL / Cosmos DB**: Transparent Data Encryption (TDE) enabled
- **Blob Storage**: Azure Storage Service Encryption (SSE) with customer-managed keys (CMK) stored in Key Vault
- **Backups**: All backups encrypted with CMK from Azure Key Vault
- **AES-256** for data at rest (DB + Blob)

### In-Transit
- All API endpoints over **HTTPS / TLS 1.2+** (TLS 1.3 preferred)
- Internal service-to-service communication over **mTLS** (mutual TLS) where possible
- Sensitive telemetry and logs masked before storage

## Key Vault

- Stores: DB connection strings, API keys, service principal secrets
- Integration: Managed Identity for all service access — no secrets in code or environment variables
- Key rotation: periodic rotation via automated Key Vault policy
- Access logging: all Key Vault access audited
- Soft-delete + purge protection enabled on all Key Vaults
- **Managed Identity first policy**: all services use Managed Identity; service principals avoided

## API Security (APIM)

- Rate limiting and throttling enforced **per tenant**
- IP filtering enabled
- JWT validation at the gateway layer before requests reach services
- API versioning: `/api/v1/...`, `/api/v2/...`
- Subscription keys required for internal and external API consumers
- Logging and alerts configured for abnormal traffic patterns

## Network Security

- **Private Endpoints**: all tenant databases and sensitive services use private endpoints (no public DB access)
- **Azure Firewall + NSGs**: subnet-level protection; broad inbound rules disallowed
- **Hub-spoke VNet design**: production and non-prod isolated
- **Standard subnets**: `snet-app`, `snet-db`, `snet-aks`, `snet-gateway`
- Static IPs for developer VPNs and partner integrations
- **Azure Front Door + WAF**: Web Application Firewall rules protect all public endpoints

## Zero Trust Framework

- **Verify explicitly**: all requests authenticated and authorized regardless of origin
- **Least privilege**: access granted only to the minimum required
- **Assume breach**: micro-segmentation per module; continuous identity verification
- No implicit trust between services — all calls authenticated

## Privacy & Data Protection Compliance

- Row-level security for shared databases
- Column-level encryption for PII and other sensitive fields
- PII anonymization in logs and telemetry — raw PII never exposed in logs
- Consent logging per tenant/user
- **Right to be Forgotten**: automated workflow (Logic App / Function App, or your equivalent) to purge a subject's data across the database, blob storage, and audit logs
- Immutable audit logs stored in write-once storage for forensics

This covers the GDPR-shaped baseline (encryption, consent, right-to-erasure, breach notification
window). If you operate in a jurisdiction with additional requirements — India's DPDP Act, CCPA,
HIPAA, or similar — layer those specific obligations (e.g. breach notification timelines, data
residency, sector-specific consent rules) on top of this baseline rather than assuming they're
already covered.

## Security Monitoring

- **SIEM (e.g. Azure Sentinel)**: ingests logs from all services, the identity provider, the gateway, and databases
- **Anomaly detection alerts**: failed login spikes, unusual geo-IP, large data exports
- **Application Insights**: tracks API usage, authentication failures, suspicious patterns
- PII anonymization enforced in all log pipelines
- Alert SLAs: Critical (P1) notify within 1 hour; High (P2) within 6 hours; Medium/Low daily digest

## Secure Coding Standards

- Input validation: FluentValidation / DataAnnotations on all DTO inputs
- SQL: parameterized queries only (EF Core / Dapper) — no string concatenation
- Exception handling: specific exceptions caught; global middleware returns `ProblemDetails` JSON
- Logging: never log secrets or raw PII; mask before writing
- Developers should be familiar with the OWASP Top 10 and secure coding fundamentals
