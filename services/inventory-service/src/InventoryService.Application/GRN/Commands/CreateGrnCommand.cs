namespace InventoryService.Application.GRN.Commands;

using InventoryService.Application.GRN.DTOs;
using MediatR;

public record CreateGrnCommand(CreateGrnRequest Request) : IRequest<GrnDetailDto>;
