namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using DTOs;
using MediatR;

public record CreateItemWarehouseMappingCommand(CreateItemWarehouseMappingRequest Request)
    : IRequest<ItemWarehouseMappingDto>;
