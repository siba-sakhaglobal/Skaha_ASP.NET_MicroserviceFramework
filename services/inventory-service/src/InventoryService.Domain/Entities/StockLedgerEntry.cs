namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class StockLedgerEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? BinId { get; set; }
    public Guid? BatchId { get; set; }
    public StockMovementType MovementType { get; set; }
    public string DocumentType { get; set; } = string.Empty;
    public Guid DocumentId { get; set; }
    public string DocumentNumber { get; set; } = string.Empty;
    public DateOnly TransactionDate { get; set; }
    public decimal InQuantity { get; set; }
    public decimal OutQuantity { get; set; }
    public decimal BalanceQuantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Value { get; set; }
    public string Uom { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public Guid CreatedBy { get; set; }
}
