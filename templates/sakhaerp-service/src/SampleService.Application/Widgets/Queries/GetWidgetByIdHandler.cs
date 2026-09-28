namespace SampleService.Application.Widgets.Queries;

using DTOs;
using SampleService.Application.Common.Interfaces;
using SampleService.Application.Widgets.Commands;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class GetWidgetByIdHandler : IRequestHandler<GetWidgetByIdQuery, WidgetDto?>
{
    private readonly ISampleServiceDbContext _db;

    public GetWidgetByIdHandler(ISampleServiceDbContext db) { _db = db; }

    public async Task<WidgetDto?> Handle(GetWidgetByIdQuery query, CancellationToken ct)
    {
        var entity = await _db.Widgets.FirstOrDefaultAsync(w => w.Id == query.Id, ct);
        return entity is null ? null : CreateWidgetHandler.MapToDto(entity);
    }
}
