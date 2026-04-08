namespace InventoryService.Domain.Entities;

public class Batch
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public DateOnly? ManufactureDate { get; set; }
    public DateOnly? ExpiryDate { get; set; }
    public int? ShelfLifeDays { get; set; }
    public Guid WarehouseId { get; set; }
    public string Status { get; set; } = "Active"; // Active / Expired / Consumed
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
}
