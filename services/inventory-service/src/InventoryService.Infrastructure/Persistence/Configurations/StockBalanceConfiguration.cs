namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class StockBalanceConfiguration : IEntityTypeConfiguration<StockBalance>
{
    public void Configure(EntityTypeBuilder<StockBalance> builder)
    {
        builder.ToTable("stock_balances");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50);
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.BinId).HasColumnName("bin_id");
        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.OnHandQuantity).HasColumnName("on_hand_quantity").HasPrecision(18, 4);
        builder.Property(e => e.ReservedQuantity).HasColumnName("reserved_quantity").HasPrecision(18, 4);
        builder.Ignore(e => e.AvailableQuantity); // Computed in code
        builder.Property(e => e.AverageCost).HasColumnName("average_cost").HasPrecision(18, 4);
        builder.Ignore(e => e.TotalValue); // Computed in code
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasIndex(e => new { e.TenantId, e.ItemId, e.WarehouseId, e.BinId, e.BatchId })
            .HasDatabaseName("idx_sb_tenant_item_wh");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
