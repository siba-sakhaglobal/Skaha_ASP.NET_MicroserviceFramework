namespace InventoryService.Api.Controllers;

using InventoryService.Application.GRN.Commands;
using InventoryService.Application.GRN.DTOs;
using InventoryService.Application.GRN.Queries;
using InventoryService.Api.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/v1/inventory/grns")]
[EnableRateLimiting(TenantRateLimitingExtensions.TenantPolicy)]
public class GrnController : ControllerBase
{
    private readonly IMediator _mediator;

    public GrnController(IMediator mediator) { _mediator = mediator; }

    [HttpGet]
    public async Task<IActionResult> GetList(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? grnType = null,
        [FromQuery] string? status = null,
        [FromQuery] string? search = null,
        [FromQuery] string? dateFrom = null,
        [FromQuery] string? dateTo = null,
        [FromQuery] Guid? siteId = null,
        CancellationToken ct = default)
    {
        var query = new GetGrnListQuery(page, pageSize, search, grnType, status, siteId, dateFrom, dateTo);
        var result = await _mediator.Send(query, ct);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetGrnByIdQuery(id), ct);
        return result is not null ? Ok(new { success = true, data = result }) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateGrnRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateGrnCommand(request), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
    }
}
