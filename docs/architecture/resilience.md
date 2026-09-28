# Resilience & Orchestration Patterns

## Saga Pattern

Used for long-running business transactions spanning multiple services (e.g. "Order → Inventory →
Payment → Shipment").

### Choreography (Event-Driven)
- Services emit and consume domain events via **Azure Service Bus / Event Grid** (or your
  provider's equivalent).
- No central orchestrator.
- Example flow:
  1. `OrderService` publishes `OrderCreated`
  2. `InventoryService` consumes → reserves stock → publishes `InventoryReserved`
  3. `PaymentService` consumes → processes payment → publishes `PaymentConfirmed`
- Pros: decentralized, no bottleneck; works well with anything subscribing to domain events
  (background workers, AI agents, analytics pipelines).
- Cons: harder to track and debug; compensation logic scattered across services.

### Orchestration (Central Coordinator)
- One orchestrator drives the workflow and invokes each service step.
- Implemented via **Azure Durable Functions** (Orchestrator Function) or a dedicated Orchestrator
  microservice.
- State persisted in Azure Storage Tables / Cosmos DB.
- Pros: easier saga + compensation implementation; full workflow visibility.
- Cons: the orchestration service becomes a dependency; must manage state persistence.

## Compensation Pattern

Used to undo/rollback when part of a distributed transaction fails (e.g. cancel an order if
payment fails).

- Each service exposes a **compensating action endpoint** (idempotent).
- The saga orchestrator calls the compensation chain on failure, in reverse order.
- Example compensation chain:
  - Payment → issue refund
  - Inventory → restock items
  - Shipment → cancel booking

## Circuit Breaker Pattern

- Prevents cascading failures when a downstream service is unavailable.
- Implementation: **Polly** (`Microsoft.Extensions.Http.Resilience`) in .NET services.
- Wrap all outbound HTTP/queue calls with: `retry policy → circuit breaker policy`.
- Central configuration in **Azure App Configuration** (thresholds shared across services), or
  your own config service.
- Circuit breaker events logged to Application Insights for monitoring.

## Retry Pattern

- Handles transient failures (DB, Service Bus, external API).
- Implementation: **Polly retry with exponential backoff** in all services.
- Azure Durable Functions: built-in retry policies for activity functions.
- Azure Service Bus: auto-retry + dead-letter queues (DLQ) for poison messages.

## Idempotency

- **All retryable API endpoints and event handlers must be idempotent.**
- Critical for financial and inventory-style workflows to prevent double-processing.
- Implementation options:
  - Unique request IDs (client-provided, stored and checked before processing).
  - Deduplication logic in event handlers (check a processed-event log).
  - Distributed locks for critical state changes.
  - State checks (if already processed, return the cached result).

## Dead-Letter Queue (DLQ) Processing Policy

- **Transient errors**: auto-retry with exponential backoff (configurable max retries).
- **Poison messages** (repeated failures after max retries):
  - Move to DLQ.
  - Alert the operations team.
  - Manual review / escalation workflow.
- DLQ messages exported periodically for analysis.
- All DLQ events logged with a correlation ID.

## Three Deployment Options for Orchestration

### Option 1 — Centralized Durable Function App
- Deploy an Azure Function App with Durable Functions.
- Each workflow = one Orchestrator Function (e.g. `OrderProcessingOrchestration`).
- State: Azure Storage Tables / Cosmos DB.
- Best for: simple orchestrations, quick start, serverless auto-scale.
- Limitation: can become an orchestration bottleneck under high load.

### Option 2 — Orchestration as a Service (AKS)
- Dedicated Orchestrator service on AKS.
- Uses: Dapr Workflow, MassTransit (Service Bus backend), or a custom orchestrator.
- Communicates via Service Bus / REST.
- Best for: complex cross-service workflows at scale.
- Limitation: more infra to manage; must handle state persistence (Cosmos DB / SQL).

### Option 3 — Distributed Choreography (Event-Driven, No Orchestrator)
- Services publish and subscribe via Event Grid / Service Bus only.
- No central coordinator.
- Best for: loosely coupled, high-volume event flows; background/AI subscriptions.
- Limitation: harder to track workflows; compensation logic must be distributed.

### Recommended Approach
- Start with **Durable Functions** for simpler workflows.
- Evolve to a **dedicated Orchestrator service** when scale demands it.
- Use **Choreography** for notification/event pipelines that are fire-and-forget.

## Example Deployment

| Pattern | Where It Typically Lives |
|---------|---------------|
| Orchestration (Durable Functions / dedicated service) | A `core`/orchestration namespace (AKS) or a dedicated Function App |
| Choreography events | Azure Service Bus + Event Grid (shared infra) |
| Retry + Circuit Breaker | Inside each service (Polly, code-level) |
| Compensation endpoints | Inside each domain service (idempotent HTTP endpoints) |

## Observability for Resilience

- All orchestration steps and retries logged with a **Correlation ID**.
- Azure Durable Functions: built-in App Insights workflow tracking (out-of-box).
- Dedicated saga workflow dashboards in App Insights / Grafana / Power BI.
- Track: compensation call success/failure, circuit breaker state transitions, DLQ depth.
