namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using DTOs;
using MediatR;

public record UpdateItemWarehouseMappingCommand(Guid Id, UpdateItemWarehouseMappingRequest Request)
    : IRequest<ItemWarehouseMappingDto>;
