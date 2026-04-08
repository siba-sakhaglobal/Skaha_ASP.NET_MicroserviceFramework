namespace InventoryService.Application.ItemWarehouseMappings.Queries;

using InventoryService.Application.ItemWarehouseMappings.DTOs;
using MediatR;

public record GetItemWarehouseMappingByIdQuery(Guid Id) : IRequest<ItemWarehouseMappingDto?>;
