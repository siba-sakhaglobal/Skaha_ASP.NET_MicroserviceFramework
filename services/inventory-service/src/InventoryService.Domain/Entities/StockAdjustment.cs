namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class StockAdjustment : AuditableEntity
{
    public string AdjustmentNumber { get; set; } = string.Empty;
    public DateOnly AdjustmentDate { get; set; }
    public string AdjustmentType { get; set; } = string.Empty; // Increase / Decrease
    public Guid WarehouseId { get; set; }
    public string? ReasonCode { get; set; }
    public StockAdjustmentStatus Status { get; set; } = StockAdjustmentStatus.Draft;
    public string? Remarks { get; set; }

    public List<StockAdjustmentLine> Lines { get; set; } = [];
}

public class StockAdjustmentLine : AuditableEntity
{
    public Guid StockAdjustmentId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public string Uom { get; set; } = string.Empty;
    public Guid? BinId { get; set; }
    public Guid? BatchId { get; set; }

    public StockAdjustment StockAdjustment { get; set; } = null!;
}
