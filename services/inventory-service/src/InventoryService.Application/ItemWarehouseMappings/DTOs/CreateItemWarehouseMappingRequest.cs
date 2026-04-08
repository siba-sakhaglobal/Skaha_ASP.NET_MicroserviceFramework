namespace InventoryService.Application.ItemWarehouseMappings.DTOs;

public record CreateItemWarehouseMappingRequest(
    Guid ItemId, string ItemCode, string ItemName,
    Guid WarehouseId, string WarehouseCode, string WarehouseName,
    Guid? WarehouseBinId, string? BinName,
    string? TraceabilityLevel,
    decimal? OpeningQuantity, decimal? OpeningRate, DateOnly? OpeningStockDate,
    decimal? MinQuantity, decimal? ReorderQuantity, decimal? MaxQuantity);
