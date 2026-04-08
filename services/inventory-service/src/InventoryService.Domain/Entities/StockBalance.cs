namespace InventoryService.Domain.Entities;

public class StockBalance : AuditableEntity
{
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? BinId { get; set; }
    public Guid? BatchId { get; set; }
    public decimal OnHandQuantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity => OnHandQuantity - ReservedQuantity;
    public decimal AverageCost { get; set; }
    public decimal TotalValue => OnHandQuantity * AverageCost;
    public string Uom { get; set; } = string.Empty;
}
