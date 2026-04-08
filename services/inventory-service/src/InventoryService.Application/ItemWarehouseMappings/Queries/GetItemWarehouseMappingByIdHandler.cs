namespace InventoryService.Application.ItemWarehouseMappings.Queries;

using InventoryService.Application.ItemWarehouseMappings.Commands;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetItemWarehouseMappingByIdHandler : IRequestHandler<GetItemWarehouseMappingByIdQuery, ItemWarehouseMappingDto?>
{
    private readonly IInventoryDbContext _db;

    public GetItemWarehouseMappingByIdHandler(IInventoryDbContext db) { _db = db; }

    public async Task<ItemWarehouseMappingDto?> Handle(GetItemWarehouseMappingByIdQuery query, CancellationToken ct)
    {
        var entity = await _db.ItemWarehouseMappings
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.Id == query.Id, ct);

        return entity is null ? null : CreateItemWarehouseMappingHandler.MapToDto(entity);
    }
}
