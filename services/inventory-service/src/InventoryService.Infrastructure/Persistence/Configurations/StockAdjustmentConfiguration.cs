namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockAdjustmentConfiguration : IEntityTypeConfiguration<StockAdjustment>
{
    public void Configure(EntityTypeBuilder<StockAdjustment> builder)
    {
        builder.ToTable("stock_adjustments");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.AdjustmentNumber).HasColumnName("adjustment_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.AdjustmentDate).HasColumnName("adjustment_date").IsRequired();
        builder.Property(e => e.AdjustmentType).HasColumnName("adjustment_type").HasMaxLength(20);
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.ReasonCode).HasColumnName("reason_code").HasMaxLength(50);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasMany(e => e.Lines).WithOne(e => e.StockAdjustment).HasForeignKey(e => e.StockAdjustmentId);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class StockAdjustmentLineConfiguration : IEntityTypeConfiguration<StockAdjustmentLine>
{
    public void Configure(EntityTypeBuilder<StockAdjustmentLine> builder)
    {
        builder.ToTable("stock_adjustment_lines");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.StockAdjustmentId).HasColumnName("stock_adjustment_id").IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50);
        builder.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(200);
        builder.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(18, 4);
        builder.Property(e => e.Rate).HasColumnName("rate").HasPrecision(18, 4);
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20);
        builder.Property(e => e.BinId).HasColumnName("bin_id");
        builder.Property(e => e.BatchId).HasColumnName("batch_id");

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
