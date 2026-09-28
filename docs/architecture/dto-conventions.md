# API & DTO Conventions

Standard patterns for all service APIs built on this framework.

## Base URL Pattern
```
/api/v{version}/{module-resource}
```
Example: `/api/v1/orders/order-lines`

## Standard Response Wrapper
```json
{
  "success": true,
  "data": { ... },
  "errors": [],
  "metadata": {
    "timestamp": "2026-01-15T10:30:00Z",
    "correlationId": "uuid"
  }
}
```

## Paginated List Response
```json
{
  "success": true,
  "data": {
    "items": [ ... ],
    "pagination": {
      "page": 1,
      "pageSize": 20,
      "totalItems": 156,
      "totalPages": 8,
      "hasNext": true,
      "hasPrevious": false
    }
  }
}
```

## Paginated List Request (Query Parameters)
| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `page` | int | 1 | Page number |
| `pageSize` | int | 20 | Items per page (max 100) |
| `sortBy` | string | `createdAt` | Column to sort by |
| `sortOrder` | string | `desc` | `asc` or `desc` |
| `search` | string | - | Global search term |
| `filter[field]` | string | - | Field-specific filter |

## Error Response (ProblemDetails RFC 7807)
```json
{
  "success": false,
  "data": null,
  "errors": [
    {
      "type": "ValidationError",
      "title": "Validation Failed",
      "status": 400,
      "detail": "Order quantity exceeds available stock",
      "field": "lines[0].quantity",
      "traceId": "uuid"
    }
  ]
}
```

## HTTP Status Codes
| Code | Usage |
|------|-------|
| `200` | Successful GET, PUT |
| `201` | Successful POST (created) |
| `204` | Successful DELETE |
| `400` | Validation error |
| `401` | Unauthenticated |
| `403` | Unauthorized (insufficient permissions) |
| `404` | Resource not found |
| `409` | Conflict (duplicate, concurrency) |
| `422` | Business rule violation |
| `500` | Internal server error |

## Standard Headers
| Header | Direction | Description |
|--------|-----------|--------------|
| `Authorization` | Request | `Bearer {jwt-token}` |
| `X-Tenant-Id` | Request | Tenant identifier (also in JWT) |
| `X-Correlation-Id` | Both | Request tracing across services |
| `X-Request-Id` | Request | Idempotency key for mutations |
| `Content-Type` | Both | `application/json` |

## Audit Fields (All Entities)
Every entity includes these audit fields:
```
createdAt: DateTimeOffset (UTC)
createdBy: UUID (userId)
modifiedAt: DateTimeOffset (UTC, nullable)
modifiedBy: UUID (nullable)
isDeleted: boolean (soft delete)
deletedAt: DateTimeOffset (nullable)
deletedBy: UUID (nullable)
```

## Naming Conventions
- **API paths**: kebab-case (`/orders`, `/order-lines`)
- **JSON fields**: camelCase (`orderNumber`, `customerId`)
- **Query params**: camelCase (`pageSize`, `sortBy`)
- **C# DTOs**: PascalCase (`OrderDto`, `CreateOrderCommand`)
- **DB columns**: snake_case (`order_number`, `customer_id`)

## Common DTO Patterns

### Create Command
```csharp
public record CreateOrderCommand(
    Guid CustomerId,
    string Notes,
    List<OrderLineDto> Lines
);
```

### Update Command
```csharp
public record UpdateOrderCommand(
    Guid Id,
    string Notes,
    List<OrderLineDto> Lines
);
```

### Query Response
```csharp
public record OrderDto(
    Guid Id,
    string OrderNumber,
    string Status,
    Guid CustomerId,
    string CustomerName,
    DateTimeOffset CreatedAt,
    string CreatedByName,
    List<OrderLineDto> Lines
);
```

### List Item (Slim)
```csharp
public record OrderListDto(
    Guid Id,
    string OrderNumber,
    string Status,
    string CustomerName,
    int LineCount,
    DateTimeOffset CreatedAt
);
```
