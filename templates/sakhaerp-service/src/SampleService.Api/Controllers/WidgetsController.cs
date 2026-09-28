namespace SampleService.Api.Controllers;

using SampleService.Application.Widgets.Commands;
using SampleService.Application.Widgets.DTOs;
using SampleService.Application.Widgets.Queries;
using SampleService.Api.RateLimiting;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

[ApiController]
[Route("api/v1/widgets")]
[EnableRateLimiting(TenantRateLimitingExtensions.TenantPolicy)]
public class WidgetsController : ControllerBase
{
    private readonly IMediator _mediator;

    public WidgetsController(IMediator mediator) { _mediator = mediator; }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetWidgetByIdQuery(id), ct);
        return result is not null ? Ok(new { success = true, data = result }) : NotFound();
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWidgetRequest request, CancellationToken ct)
    {
        var result = await _mediator.Send(new CreateWidgetCommand(request), ct);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, new { success = true, data = result });
    }
}
