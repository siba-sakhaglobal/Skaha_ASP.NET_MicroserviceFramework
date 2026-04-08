namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using DTOs;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateItemWarehouseMappingHandler : IRequestHandler<CreateItemWarehouseMappingCommand, ItemWarehouseMappingDto>
{
    private readonly IInventoryDbContext _db;

    public CreateItemWarehouseMappingHandler(IInventoryDbContext db) { _db = db; }

    public async Task<ItemWarehouseMappingDto> Handle(CreateItemWarehouseMappingCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // Check duplicate
        var exists = await _db.ItemWarehouseMappings
            .AnyAsync(m => m.ItemId == req.ItemId && m.WarehouseId == req.WarehouseId, ct);
        if (exists)
            throw new InvalidOperationException("Item-Warehouse mapping already exists.");

        var entity = new ItemWarehouseMapping
        {
            MappingCode = $"IWM-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..8].ToUpper()}",
            ItemId = req.ItemId,
            ItemCode = req.ItemCode,
            ItemName = req.ItemName,
            WarehouseId = req.WarehouseId,
            WarehouseCode = req.WarehouseCode,
            WarehouseName = req.WarehouseName,
            WarehouseBinId = req.WarehouseBinId,
            BinName = req.BinName,
            TraceabilityLevel = Enum.TryParse<TraceabilityLevel>(req.TraceabilityLevel, true, out var tl) ? tl : TraceabilityLevel.None,
            OpeningQuantity = req.OpeningQuantity,
            OpeningRate = req.OpeningRate,
            OpeningStockDate = req.OpeningStockDate,
            MinQuantity = req.MinQuantity,
            ReorderQuantity = req.ReorderQuantity,
            MaxQuantity = req.MaxQuantity,
            Status = MappingStatus.Active
        };

        _db.ItemWarehouseMappings.Add(entity);
        await _db.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    internal static ItemWarehouseMappingDto MapToDto(ItemWarehouseMapping e) => new(
        e.Id, e.MappingCode, e.ItemId, e.ItemCode, e.ItemName,
        e.WarehouseId, e.WarehouseCode, e.WarehouseName,
        e.WarehouseBinId, e.BinName, e.Status.ToString(), e.TraceabilityLevel.ToString(),
        e.OpeningQuantity, e.OpeningRate, e.OpeningStockValue, e.OpeningStockDate,
        e.MinQuantity, e.ReorderQuantity, e.MaxQuantity, e.CreatedAt);
}
