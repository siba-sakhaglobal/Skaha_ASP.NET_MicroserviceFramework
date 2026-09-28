namespace SampleService.Application.Widgets.DTOs;

public record WidgetDto(
    Guid Id, string Code, string Name, decimal Quantity, DateTimeOffset CreatedAt);
