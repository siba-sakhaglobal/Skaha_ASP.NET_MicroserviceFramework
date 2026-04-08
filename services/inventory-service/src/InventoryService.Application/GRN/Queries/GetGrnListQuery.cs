namespace InventoryService.Application.GRN.Queries;

using InventoryService.Application.Common.Models;
using InventoryService.Application.GRN.DTOs;
using MediatR;

public record GetGrnListQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? GrnType = null,
    string? Status = null,
    Guid? SiteId = null,
    string? DateFrom = null,
    string? DateTo = null) : IRequest<PagedResponse<GrnListDto>>;
