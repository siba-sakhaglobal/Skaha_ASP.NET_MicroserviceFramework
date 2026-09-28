namespace SakhaERP.Shared.EventContracts.Inventory;

public record StockTransferDispatchedEvent : IntegrationEvent
{
    public Guid TransferId { get; init; }
    public Guid SourceWarehouseId { get; init; }
    public IReadOnlyList<TransferItemInfo> Items { get; init; } = [];
}

public record TransferItemInfo(Guid ItemId, string ItemCode, decimal Quantity);
