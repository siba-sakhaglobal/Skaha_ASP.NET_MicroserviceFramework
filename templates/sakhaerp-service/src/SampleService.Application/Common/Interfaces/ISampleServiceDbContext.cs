namespace SampleService.Application.Common.Interfaces;

using SampleService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface ISampleServiceDbContext
{
    DbSet<Widget> Widgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
