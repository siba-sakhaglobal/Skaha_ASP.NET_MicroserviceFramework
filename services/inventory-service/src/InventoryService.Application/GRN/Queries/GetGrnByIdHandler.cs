namespace InventoryService.Application.GRN.Queries;

using InventoryService.Application.GRN.Commands;
using InventoryService.Application.GRN.DTOs;
using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetGrnByIdHandler : IRequestHandler<GetGrnByIdQuery, GrnDetailDto?>
{
    private readonly IInventoryDbContext _db;

    public GetGrnByIdHandler(IInventoryDbContext db) { _db = db; }

    public async Task<GrnDetailDto?> Handle(GetGrnByIdQuery query, CancellationToken ct)
    {
        var grn = await _db.GoodsReceivedNotes
            .AsNoTracking()
            .Include(g => g.LineItems)
            .FirstOrDefaultAsync(g => g.Id == query.Id, ct);

        return grn is null ? null : CreateGrnHandler.MapToDetailDto(grn);
    }
}
