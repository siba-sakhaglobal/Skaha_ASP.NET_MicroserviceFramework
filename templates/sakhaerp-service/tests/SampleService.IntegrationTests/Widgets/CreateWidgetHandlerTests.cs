namespace SampleService.IntegrationTests.Widgets;

using FluentAssertions;
using SampleService.Application.Widgets.Commands;
using SampleService.Application.Widgets.DTOs;
using SampleService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class CreateWidgetHandlerTests
{
    private static SampleServiceDbContext NewInMemoryContext()
    {
        var options = new DbContextOptionsBuilder<SampleServiceDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        // Fallback (tenantId = Guid.Empty, isSharedDb = true) constructor skips the tenant
        // query filter, which keeps this test focused on the handler's own behavior.
        return new SampleServiceDbContext(options);
    }

    [Fact]
    public async Task Handle_withNewCode_persistsWidgetAndReturnsDto()
    {
        await using var db = NewInMemoryContext();
        var handler = new CreateWidgetHandler(db);

        var result = await handler.Handle(
            new CreateWidgetCommand(new CreateWidgetRequest("WGT-001", "Widget", 10m)),
            CancellationToken.None);

        result.Code.Should().Be("WGT-001");
        (await db.Widgets.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task Handle_withDuplicateCode_throwsInvalidOperationException()
    {
        await using var db = NewInMemoryContext();
        var handler = new CreateWidgetHandler(db);
        var request = new CreateWidgetRequest("WGT-001", "Widget", 10m);

        await handler.Handle(new CreateWidgetCommand(request), CancellationToken.None);

        var act = async () => await handler.Handle(new CreateWidgetCommand(request), CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>();
    }
}
