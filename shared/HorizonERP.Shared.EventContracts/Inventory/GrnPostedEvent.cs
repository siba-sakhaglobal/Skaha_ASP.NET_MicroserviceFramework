namespace HorizonERP.Shared.EventContracts.Inventory;

public record GrnPostedEvent : IntegrationEvent
{
    public Guid GrnId { get; init; }
    public Guid WarehouseId { get; init; }
    public Guid? PurchaseOrderId { get; init; }
    public IReadOnlyList<GrnItemInfo> Items { get; init; } = [];
}

public record GrnItemInfo(Guid ItemId, string ItemCode, decimal Quantity, string Uom);
