namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ItemWarehouseMappingConfiguration : IEntityTypeConfiguration<ItemWarehouseMapping>
{
    public void Configure(EntityTypeBuilder<ItemWarehouseMapping> builder)
    {
        builder.ToTable("item_warehouse_mappings");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.MappingCode).HasColumnName("mapping_code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(200);
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.WarehouseCode).HasColumnName("warehouse_code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.WarehouseName).HasColumnName("warehouse_name").HasMaxLength(200);
        builder.Property(e => e.WarehouseBinId).HasColumnName("warehouse_bin_id");
        builder.Property(e => e.BinName).HasColumnName("bin_name").HasMaxLength(100);
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.TraceabilityLevel).HasColumnName("traceability_level").HasConversion<string>().HasMaxLength(20);
        builder.Property(e => e.OpeningQuantity).HasColumnName("opening_quantity").HasPrecision(18, 4);
        builder.Property(e => e.OpeningRate).HasColumnName("opening_rate").HasPrecision(18, 4);
        builder.Ignore(e => e.OpeningStockValue); // Computed in code
        builder.Property(e => e.OpeningStockDate).HasColumnName("opening_stock_date");
        builder.Property(e => e.MinQuantity).HasColumnName("min_quantity").HasPrecision(18, 4);
        builder.Property(e => e.ReorderQuantity).HasColumnName("reorder_quantity").HasPrecision(18, 4);
        builder.Property(e => e.MaxQuantity).HasColumnName("max_quantity").HasPrecision(18, 4);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Indexes
        builder.HasIndex(e => new { e.TenantId, e.ItemId, e.WarehouseId })
            .IsUnique()
            .HasFilter("is_deleted = false")
            .HasDatabaseName("idx_iwm_unique");
        builder.HasIndex(e => new { e.TenantId, e.ItemId }).HasDatabaseName("idx_iwm_tenant_item");
        builder.HasIndex(e => new { e.TenantId, e.WarehouseId }).HasDatabaseName("idx_iwm_tenant_wh");

        // Query filter for soft delete
        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
