namespace InventoryService.UnitTests.ItemWarehouseMappings;

using FluentAssertions;
using InventoryService.Application.ItemWarehouseMappings.Commands;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using InventoryService.Application.ItemWarehouseMappings.Validators;

public class CreateItemWarehouseMappingValidatorTests
{
    private readonly CreateItemWarehouseMappingValidator _validator = new();

    private static CreateItemWarehouseMappingRequest ValidRequest() => new(
        ItemId: Guid.NewGuid(), ItemCode: "ITM-001", ItemName: "Widget",
        WarehouseId: Guid.NewGuid(), WarehouseCode: "WH-001", WarehouseName: "Main Warehouse",
        WarehouseBinId: null, BinName: null,
        TraceabilityLevel: null,
        OpeningQuantity: null, OpeningRate: null, OpeningStockDate: null,
        MinQuantity: null, ReorderQuantity: null, MaxQuantity: null);

    [Fact]
    public void Validate_withValidRequest_passesWithNoErrors()
    {
        var command = new CreateItemWarehouseMappingCommand(ValidRequest());

        var result = _validator.Validate(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_withEmptyItemId_failsOnItemId()
    {
        var command = new CreateItemWarehouseMappingCommand(ValidRequest() with { ItemId = Guid.Empty });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.ItemId");
    }

    [Fact]
    public void Validate_withOpeningQuantityButNoOpeningRate_failsWithOpeningRateRequiredMessage()
    {
        var command = new CreateItemWarehouseMappingCommand(ValidRequest() with { OpeningQuantity = 10m, OpeningRate = null });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Request.OpeningRate" &&
            e.ErrorMessage == "Opening rate is required when opening quantity > 0");
    }

    [Fact]
    public void Validate_withMaxQuantityLessThanMinQuantity_failsWithMaxQuantityMessage()
    {
        var command = new CreateItemWarehouseMappingCommand(ValidRequest() with { MinQuantity = 100m, MaxQuantity = 50m });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == "Request.MaxQuantity" &&
            e.ErrorMessage == "Max quantity must be >= min quantity");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-0.01)]
    public void Validate_withNegativeMinQuantity_failsOnMinQuantity(decimal minQuantity)
    {
        var command = new CreateItemWarehouseMappingCommand(ValidRequest() with { MinQuantity = minQuantity });

        var result = _validator.Validate(command);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Request.MinQuantity");
    }
}
