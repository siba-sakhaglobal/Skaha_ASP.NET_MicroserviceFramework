namespace InventoryService.Application.ItemWarehouseMappings.Queries;

using InventoryService.Application.Common.Models;
using InventoryService.Application.ItemWarehouseMappings.Commands;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using InventoryService.Domain.Enums;
using InventoryService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetItemWarehouseMappingListHandler
    : IRequestHandler<GetItemWarehouseMappingListQuery, PagedResponse<ItemWarehouseMappingDto>>
{
    private readonly IInventoryDbContext _db;

    public GetItemWarehouseMappingListHandler(IInventoryDbContext db) { _db = db; }

    public async Task<PagedResponse<ItemWarehouseMappingDto>> Handle(
        GetItemWarehouseMappingListQuery query, CancellationToken ct)
    {
        var q = _db.ItemWarehouseMappings.AsNoTracking().AsQueryable();

        // Filter by ItemId
        if (query.ItemId.HasValue)
            q = q.Where(m => m.ItemId == query.ItemId.Value);

        // Filter by WarehouseId
        if (query.WarehouseId.HasValue)
            q = q.Where(m => m.WarehouseId == query.WarehouseId.Value);

        // Filter by Status
        if (!string.IsNullOrWhiteSpace(query.Status) && Enum.TryParse<MappingStatus>(query.Status, true, out var status))
            q = q.Where(m => m.Status == status);

        // Search on ItemCode, WarehouseName
        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            q = q.Where(m =>
                m.ItemCode.ToLower().Contains(search) ||
                m.ItemName.ToLower().Contains(search) ||
                m.WarehouseName.ToLower().Contains(search) ||
                m.WarehouseCode.ToLower().Contains(search));
        }

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(m => m.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync(ct);

        var dtos = items.Select(CreateItemWarehouseMappingHandler.MapToDto).ToList();

        return new PagedResponse<ItemWarehouseMappingDto>(dtos, totalCount, query.Page, query.PageSize);
    }
}
