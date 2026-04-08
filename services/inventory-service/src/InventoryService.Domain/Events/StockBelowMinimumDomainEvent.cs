namespace InventoryService.Domain.Events;

public record StockBelowMinimumDomainEvent(
    Guid ItemId,
    Guid WarehouseId,
    decimal CurrentQty,
    decimal MinQty,
    Guid TenantId,
    DateTimeOffset OccurredAt);
