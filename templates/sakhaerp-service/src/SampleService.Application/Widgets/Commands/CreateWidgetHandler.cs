namespace SampleService.Application.Widgets.Commands;

using DTOs;
using SampleService.Domain.Entities;
using SampleService.Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateWidgetHandler : IRequestHandler<CreateWidgetCommand, WidgetDto>
{
    private readonly ISampleServiceDbContext _db;

    public CreateWidgetHandler(ISampleServiceDbContext db) { _db = db; }

    public async Task<WidgetDto> Handle(CreateWidgetCommand command, CancellationToken ct)
    {
        var req = command.Request;

        var exists = await _db.Widgets.AnyAsync(w => w.Code == req.Code, ct);
        if (exists)
            throw new InvalidOperationException($"A widget with code '{req.Code}' already exists.");

        var entity = new Widget
        {
            Code = req.Code,
            Name = req.Name,
            Quantity = req.Quantity,
        };

        _db.Widgets.Add(entity);
        await _db.SaveChangesAsync(ct);

        return MapToDto(entity);
    }

    internal static WidgetDto MapToDto(Widget e) => new(e.Id, e.Code, e.Name, e.Quantity, e.CreatedAt);
}
