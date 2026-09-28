namespace SampleService.Application.Widgets.Queries;

using DTOs;
using MediatR;

public record GetWidgetByIdQuery(Guid Id) : IRequest<WidgetDto?>;
