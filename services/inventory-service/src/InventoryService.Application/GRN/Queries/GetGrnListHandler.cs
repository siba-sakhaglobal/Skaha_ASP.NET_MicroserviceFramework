namespace InventoryService.Application.GRN.Queries;

using InventoryService.Application.Common.Models;
using InventoryService.Application.GRN.DTOs;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetGrnListHandler : IRequestHandler<GetGrnListQuery, PagedResponse<GrnListDto>>
{
    private readonly IInventoryDbContext _db;

    public GetGrnListHandler(IInventoryDbContext db) { _db = db; }

    public async Task<PagedResponse<GrnListDto>> Handle(GetGrnListQuery query, CancellationToken ct)
    {
        var q = _db.GoodsReceivedNotes
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.GrnType) &&
            Enum.TryParse<GrnType>(query.GrnType, true, out var grnType))
            q = q.Where(g => g.GrnType == grnType);

        if (!string.IsNullOrWhiteSpace(query.Status) &&
            Enum.TryParse<GrnStatus>(query.Status, true, out var status))
            q = q.Where(g => g.Status == status);

        if (query.SiteId.HasValue)
            q = q.Where(g => g.SiteId == query.SiteId.Value);

        if (!string.IsNullOrWhiteSpace(query.DateFrom) &&
            DateOnly.TryParse(query.DateFrom, out var dateFrom))
            q = q.Where(g => g.GrnDate >= dateFrom);

        if (!string.IsNullOrWhiteSpace(query.DateTo) &&
            DateOnly.TryParse(query.DateTo, out var dateTo))
            q = q.Where(g => g.GrnDate <= dateTo);

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower();
            q = q.Where(g =>
                g.GrnNumber.ToLower().Contains(search) ||
                (g.VendorName != null && g.VendorName.ToLower().Contains(search)) ||
                (g.CustomerName != null && g.CustomerName.ToLower().Contains(search)) ||
                g.SiteName.ToLower().Contains(search));
        }

        var totalCount = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(g => g.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(g => new GrnListDto(
                g.Id,
                g.GrnNumber,
                g.GrnDate.ToString("yyyy-MM-dd"),
                g.GrnType.ToString(),
                g.Status.ToString(),
                g.SiteName,
                g.WarehouseName,
                g.VendorName,
                g.CustomerName,
                g.TotalGrnValue,
                g.LineItems.Count,
                g.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ")))
            .ToListAsync(ct);

        return new PagedResponse<GrnListDto>(items, totalCount, query.Page, query.PageSize);
    }
}
