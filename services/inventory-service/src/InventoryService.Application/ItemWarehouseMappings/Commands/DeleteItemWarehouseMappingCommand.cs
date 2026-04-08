namespace InventoryService.Application.ItemWarehouseMappings.Commands;

using MediatR;

public record DeleteItemWarehouseMappingCommand(Guid Id) : IRequest;
