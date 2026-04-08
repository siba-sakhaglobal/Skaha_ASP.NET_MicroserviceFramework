namespace InventoryService.Application.Common.Models;

public record PagedRequest(int Page = 1, int PageSize = 20, string? Search = null)
{
    public int Skip => (Page - 1) * PageSize;
}
