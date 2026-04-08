namespace InventoryService.Application.ItemWarehouseMappings.DTOs;

public record UpdateItemWarehouseMappingRequest(
    Guid? WarehouseBinId, string? BinName, string? Status,
    decimal? MinQuantity, decimal? ReorderQuantity, decimal? MaxQuantity);
