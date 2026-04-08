namespace InventoryService.Domain.Enums;

public enum StockTransferStatus
{
    Draft,
    Approved,
    Dispatched,
    InTransit,
    Received,
    Completed,
    Cancelled
}
