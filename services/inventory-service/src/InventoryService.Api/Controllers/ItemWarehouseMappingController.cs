namespace InventoryService.Api.Controllers;

using InventoryService.Application.ItemWarehouseMappings.Commands;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using InventoryService.Application.ItemWarehouseMappings.Queries;
using InventoryService.Api.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/v1/inventory/item-warehouse-mappings")]
[EnableRateLimiting(TenantRateLimitingExtensions.TenantPolicy)]
public class ItemWarehouseMappingController : ControllerBase
{
    private readonly IMediator _mediator;

    public ItemWarehouseMappingController(IMediator mediator) { _mediator = mediator; }

    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetItemWarehouseMappingListQuery query, CancellationToken ct)
    {
        var result = await _mediator.Send(query, ct);
        return Ok(new { success = true, data = result });
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetItemWarehouseMappingByIdQuery(id), ct);
        return result is not null ? Ok(new { success = true, data = result }) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateItemWarehouseMappingRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateItemWarehouseMappingCommand(request), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateItemWarehouseMappingRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new UpdateItemWarehouseMappingCommand(id, request), ct);
        return Ok(new { success = true, data = result });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        await _mediator.Send(new DeleteItemWarehouseMappingCommand(id), ct);
        return NoContent();
    }
}
