namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class ItemWarehouseMapping : AuditableEntity
{
    public string MappingCode { get; set; } = string.Empty;
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;      // Denormalized
    public string ItemName { get; set; } = string.Empty;      // Denormalized
    public Guid WarehouseId { get; set; }
    public string WarehouseCode { get; set; } = string.Empty;  // Denormalized
    public string WarehouseName { get; set; } = string.Empty;  // Denormalized
    public Guid? WarehouseBinId { get; set; }
    public string? BinName { get; set; }
    public MappingStatus Status { get; set; } = MappingStatus.Active;
    public TraceabilityLevel TraceabilityLevel { get; set; } = TraceabilityLevel.None;

    // Opening Stock
    public decimal? OpeningQuantity { get; set; }
    public decimal? OpeningRate { get; set; }
    public decimal? OpeningStockValue => OpeningQuantity.HasValue && OpeningRate.HasValue
        ? OpeningQuantity.Value * OpeningRate.Value : null;
    public DateOnly? OpeningStockDate { get; set; }

    // Planning Thresholds
    public decimal? MinQuantity { get; set; }
    public decimal? ReorderQuantity { get; set; }
    public decimal? MaxQuantity { get; set; }
}
