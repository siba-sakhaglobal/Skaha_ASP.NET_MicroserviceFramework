namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using DTOs;
using InventoryService.Domain.Enums;
using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class UpdateItemWarehouseMappingHandler : IRequestHandler<UpdateItemWarehouseMappingCommand, ItemWarehouseMappingDto>
{
    private readonly IInventoryDbContext _db;

    public UpdateItemWarehouseMappingHandler(IInventoryDbContext db) { _db = db; }

    public async Task<ItemWarehouseMappingDto> Handle(UpdateItemWarehouseMappingCommand command, CancellationToken ct)
    {
        var entity = await _db.ItemWarehouseMappings
            .FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new InvalidOperationException($"Item-Warehouse mapping with Id '{command.Id}' not found.");

        var req = command.Request;

        if (req.WarehouseBinId.HasValue)
            entity.WarehouseBinId = req.WarehouseBinId;

        if (req.BinName is not null)
            entity.BinName = req.BinName;

        if (req.Status is not null && Enum.TryParse<MappingStatus>(req.Status, true, out var status))
            entity.Status = status;

        if (req.MinQuantity.HasValue)
            entity.MinQuantity = req.MinQuantity;

        if (req.ReorderQuantity.HasValue)
            entity.ReorderQuantity = req.ReorderQuantity;

        if (req.MaxQuantity.HasValue)
            entity.MaxQuantity = req.MaxQuantity;

        await _db.SaveChangesAsync(ct);

        return CreateItemWarehouseMappingHandler.MapToDto(entity);
    }
}
