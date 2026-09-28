namespace SampleService.UnitTests.Widgets;

using FluentAssertions;
using SampleService.Application.Widgets.Commands;
using SampleService.Application.Widgets.DTOs;
using SampleService.Application.Widgets.Validators;

public class CreateWidgetValidatorTests
{
    private readonly CreateWidgetValidator _validator = new();

    [Fact]
    public void Validate_withValidRequest_passesWithNoErrors()
    {
        var command = new CreateWidgetCommand(new CreateWidgetRequest("WGT-001", "Widget", 10m));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_withEmptyCode_failsOnCode()
    {
        var command = new CreateWidgetCommand(new CreateWidgetRequest("", "Widget", 10m));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Code");
    }

    [Fact]
    public void Validate_withNegativeQuantity_failsOnQuantity()
    {
        var command = new CreateWidgetCommand(new CreateWidgetRequest("WGT-001", "Widget", -1m));

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.Quantity");
    }
}
