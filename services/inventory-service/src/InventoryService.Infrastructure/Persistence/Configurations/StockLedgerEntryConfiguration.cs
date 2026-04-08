namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockLedgerEntryConfiguration : IEntityTypeConfiguration<StockLedgerEntry>
{
    public void Configure(EntityTypeBuilder<StockLedgerEntry> builder)
    {
        builder.ToTable("stock_ledger_entries");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50);
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.BinId).HasColumnName("bin_id");
        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.MovementType).HasColumnName("movement_type").HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.DocumentType).HasColumnName("document_type").HasMaxLength(50);
        builder.Property(e => e.DocumentId).HasColumnName("document_id");
        builder.Property(e => e.DocumentNumber).HasColumnName("document_number").HasMaxLength(50);
        builder.Property(e => e.TransactionDate).HasColumnName("transaction_date").IsRequired();
        builder.Property(e => e.InQuantity).HasColumnName("in_quantity").HasPrecision(18, 4);
        builder.Property(e => e.OutQuantity).HasColumnName("out_quantity").HasPrecision(18, 4);
        builder.Property(e => e.BalanceQuantity).HasColumnName("balance_quantity").HasPrecision(18, 4);
        builder.Property(e => e.Rate).HasColumnName("rate").HasPrecision(18, 4);
        builder.Property(e => e.Value).HasColumnName("value").HasPrecision(18, 4);
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20);
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();

        builder.HasIndex(e => new { e.TenantId, e.ItemId, e.WarehouseId, e.TransactionDate })
            .HasDatabaseName("idx_sle_tenant_item_wh_date");
    }
}
