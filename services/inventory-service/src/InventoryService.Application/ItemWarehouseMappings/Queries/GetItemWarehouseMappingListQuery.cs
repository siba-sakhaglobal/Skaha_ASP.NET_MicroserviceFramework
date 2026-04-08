namespace InventoryService.Application.ItemWarehouseMappings.Queries;

using InventoryService.Application.Common.Models;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using MediatR;

public record GetItemWarehouseMappingListQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    Guid? ItemId = null,
    Guid? WarehouseId = null,
    string? Status = null) : IRequest<PagedResponse<ItemWarehouseMappingDto>>;
