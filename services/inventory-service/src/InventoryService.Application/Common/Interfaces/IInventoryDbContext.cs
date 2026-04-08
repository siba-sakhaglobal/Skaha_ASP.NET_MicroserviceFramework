namespace InventoryService.Application.Common.Interfaces;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

public interface IInventoryDbContext
{
    DbSet<ItemWarehouseMapping> ItemWarehouseMappings { get; }
    DbSet<GateInward> GateInwards { get; }
    DbSet<GateInwardItem> GateInwardItems { get; }
    DbSet<GoodsReceivedNote> GoodsReceivedNotes { get; }
    DbSet<GrnLineItem> GrnLineItems { get; }
    DbSet<StockBalance> StockBalances { get; }
    DbSet<StockLedgerEntry> StockLedgerEntries { get; }
    DbSet<StockTransfer> StockTransfers { get; }
    DbSet<StockTransferLine> StockTransferLines { get; }
    DbSet<StockAdjustment> StockAdjustments { get; }
    DbSet<StockAdjustmentLine> StockAdjustmentLines { get; }
    DbSet<Batch> Batches { get; }
    DbSet<SerialNumber> SerialNumbers { get; }
    DbSet<InventorySettings> InventorySettings { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
