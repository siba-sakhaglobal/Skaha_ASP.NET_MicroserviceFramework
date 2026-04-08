namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class StockTransfer : AuditableEntity
{
    public string TransferNumber { get; set; } = string.Empty;
    public DateOnly TransferDate { get; set; }
    public Guid SourceWarehouseId { get; set; }
    public string SourceWarehouseName { get; set; } = string.Empty;
    public Guid DestinationWarehouseId { get; set; }
    public string DestinationWarehouseName { get; set; } = string.Empty;
    public StockTransferStatus Status { get; set; } = StockTransferStatus.Draft;
    public string? Remarks { get; set; }

    public List<StockTransferLine> Lines { get; set; } = [];
}

public class StockTransferLine : AuditableEntity
{
    public Guid StockTransferId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal RequestedQuantity { get; set; }
    public decimal DispatchedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string Uom { get; set; } = string.Empty;
    public Guid? BatchId { get; set; }
    public Guid? SourceBinId { get; set; }
    public Guid? DestBinId { get; set; }

    public StockTransfer StockTransfer { get; set; } = null!;
}
