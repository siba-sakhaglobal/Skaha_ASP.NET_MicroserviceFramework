namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockTransferConfiguration : IEntityTypeConfiguration<StockTransfer>
{
    public void Configure(EntityTypeBuilder<StockTransfer> builder)
    {
        builder.ToTable("stock_transfers");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.TransferNumber).HasColumnName("transfer_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.TransferDate).HasColumnName("transfer_date").IsRequired();
        builder.Property(e => e.SourceWarehouseId).HasColumnName("source_warehouse_id").IsRequired();
        builder.Property(e => e.SourceWarehouseName).HasColumnName("source_warehouse_name").HasMaxLength(200);
        builder.Property(e => e.DestinationWarehouseId).HasColumnName("destination_warehouse_id").IsRequired();
        builder.Property(e => e.DestinationWarehouseName).HasColumnName("destination_warehouse_name").HasMaxLength(200);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasMany(e => e.Lines).WithOne(e => e.StockTransfer).HasForeignKey(e => e.StockTransferId);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class StockTransferLineConfiguration : IEntityTypeConfiguration<StockTransferLine>
{
    public void Configure(EntityTypeBuilder<StockTransferLine> builder)
    {
        builder.ToTable("stock_transfer_lines");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.StockTransferId).HasColumnName("stock_transfer_id").IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50);
        builder.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(200);
        builder.Property(e => e.RequestedQuantity).HasColumnName("requested_quantity").HasPrecision(18, 4);
        builder.Property(e => e.DispatchedQuantity).HasColumnName("dispatched_quantity").HasPrecision(18, 4);
        builder.Property(e => e.ReceivedQuantity).HasColumnName("received_quantity").HasPrecision(18, 4);
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20);
        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.SourceBinId).HasColumnName("source_bin_id");
        builder.Property(e => e.DestBinId).HasColumnName("dest_bin_id");

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
