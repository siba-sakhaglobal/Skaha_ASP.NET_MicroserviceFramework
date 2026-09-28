namespace InventoryService.IntegrationTests.ItemWarehouseMappings;

using FluentAssertions;
using InventoryService.Application.ItemWarehouseMappings.Commands;
using InventoryService.Application.ItemWarehouseMappings.DTOs;
using InventoryService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CreateItemWarehouseMappingHandlerTests
{
    private static InventoryDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        // Fallback (tenantId = Guid.Empty, isSharedDb = true) constructor skips the tenant
        // query filter, which keeps this test focused on the handler's own behavior.
        return new InventoryDbContext(options);
    }

    [Fact]
    public async Task Handle_withNewItemWarehousePair_persistsMappingAndReturnsDto()
    {
        await using var db = NewInMemoryContext();
        var handler = new CreateItemWarehouseMappingHandler(db);
        var request = new CreateItemWarehouseMappingRequest(
            ItemId: Guid.NewGuid(), ItemCode: "ITM-001", ItemName: "Widget",
            WarehouseId: Guid.NewGuid(), WarehouseCode: "WH-001", WarehouseName: "Main Warehouse",
            WarehouseBinId: null, BinName: null,
            TraceabilityLevel: null,
            OpeningQuantity: 10m, OpeningRate: 25.5m, OpeningStockDate: DateOnly.FromDateTime(DateTime.UtcNow),
            MinQuantity: 5m, ReorderQuantity: 10m, MaxQuantity: 100m);

        ItemWarehouseMappingDto result = await handler.Handle(new CreateItemWarehouseMappingCommand(request), CancellationToken.None);

        result.MappingCode.Should().StartWith("IWM-");
        result.Status.Should().Be("Active");
        (await db.ItemWarehouseMappings.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_withDuplicateItemWarehousePair_throwsInvalidOperationException()
    {
        await using var db = NewInMemoryContext();
        var handler = new CreateItemWarehouseMappingHandler(db);
        var itemId = Guid.NewGuid();
        var warehouseId = Guid.NewGuid();
        var request = new CreateItemWarehouseMappingRequest(
            ItemId: itemId, ItemCode: "ITM-001", ItemName: "Widget",
            WarehouseId: warehouseId, WarehouseCode: "WH-001", WarehouseName: "Main Warehouse",
            WarehouseBinId: null, BinName: null,
            TraceabilityLevel: null,
            OpeningQuantity: null, OpeningRate: null, OpeningStockDate: null,
            MinQuantity: null, ReorderQuantity: null, MaxQuantity: null);

        await handler.Handle(new CreateItemWarehouseMappingCommand(request), CancellationToken.None);

        var act = async () => await handler.Handle(new CreateItemWarehouseMappingCommand(request), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
