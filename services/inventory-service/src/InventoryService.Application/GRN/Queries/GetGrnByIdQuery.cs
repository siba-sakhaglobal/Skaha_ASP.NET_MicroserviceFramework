namespace InventoryService.Application.GRN.Queries;

using InventoryService.Application.GRN.DTOs;
using MediatR;

public record GetGrnByIdQuery(Guid Id) : IRequest<GrnDetailDto?>;
