namespace InventoryService.Application.ItemWarehouseMappings.DTOs;

public record ItemWarehouseMappingDto(
    Guid Id, string MappingCode, Guid ItemId, string ItemCode, string ItemName,
    Guid WarehouseId, string WarehouseCode, string WarehouseName,
    Guid? WarehouseBinId, string? BinName, string Status, string TraceabilityLevel,
    decimal? OpeningQuantity, decimal? OpeningRate, decimal? OpeningStockValue, DateOnly? OpeningStockDate,
    decimal? MinQuantity, decimal? ReorderQuantity, decimal? MaxQuantity,
    DateTimeOffset CreatedAt);
