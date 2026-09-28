namespace SampleService.Application.Widgets.Commands;

using DTOs;
using MediatR;

public record CreateWidgetCommand(CreateWidgetRequest Request) : IRequest<WidgetDto>;
