namespace SakhaERP.Shared.EventContracts.Inventory;

public record StockBelowMinimumEvent : IntegrationEvent
{
    public Guid ItemId { get; init; }
    public string ItemCode { get; init; } = string.Empty;
    public Guid WarehouseId { get; init; }
    public decimal CurrentQuantity { get; init; }
    public decimal MinimumQuantity { get; init; }
}
