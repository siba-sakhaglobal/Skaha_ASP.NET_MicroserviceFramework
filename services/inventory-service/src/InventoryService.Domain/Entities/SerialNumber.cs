namespace InventoryService.Domain.Entities;

public class SerialNumber
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public string SerialNo { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public Guid? BatchId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinId { get; set; }
    public string Status { get; set; } = "Available"; // Available / Reserved / Issued / Consumed / Scrapped
    public string? CurrentDocType { get; set; }
    public Guid? CurrentDocId { get; set; }
}
