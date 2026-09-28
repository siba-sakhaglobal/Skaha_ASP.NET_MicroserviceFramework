namespace SampleService.Domain.Entities;

/// <summary>
/// Placeholder entity demonstrating the framework's conventions. Replace with your
/// service's real aggregate(s) — rename the folder, this file, and everything downstream
/// that references "Widget".
/// </summary>
public class Widget : AuditableEntity
{
    public string Code { get; set; } = default!;
    public string Name { get; set; } = default!;
    public decimal Quantity { get; set; }
}
