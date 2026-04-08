namespace InventoryService.Domain.Events;

public record GrnPostedDomainEvent(Guid GrnId, Guid TenantId, DateTimeOffset OccurredAt);
