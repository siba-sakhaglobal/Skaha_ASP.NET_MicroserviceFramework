namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class DeleteItemWarehouseMappingHandler : IRequestHandler<DeleteItemWarehouseMappingCommand>
{
    private readonly IInventoryDbContext _db;

    public DeleteItemWarehouseMappingHandler(IInventoryDbContext db) { _db = db; }

    public async Task Handle(DeleteItemWarehouseMappingCommand command, CancellationToken ct)
    {
        var entity = await _db.ItemWarehouseMappings
            .FirstOrDefaultAsync(m => m.Id == command.Id, ct)
            ?? throw new InvalidOperationException($"Item-Warehouse mapping with Id '{command.Id}' not found.");

        entity.IsDeleted = true;
        await _db.SaveChangesAsync(ct);
    }
}
