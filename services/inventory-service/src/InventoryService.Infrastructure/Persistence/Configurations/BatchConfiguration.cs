namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class BatchConfiguration : IEntityTypeConfiguration<Batch>
{
    public void Configure(EntityTypeBuilder<Batch> builder)
    {
        builder.ToTable("batches");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.BatchNumber).HasColumnName("batch_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ManufactureDate).HasColumnName("manufacture_date");
        builder.Property(e => e.ExpiryDate).HasColumnName("expiry_date");
        builder.Property(e => e.ShelfLifeDays).HasColumnName("shelf_life_days");
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
        builder.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(18, 4);
        builder.Property(e => e.Rate).HasColumnName("rate").HasPrecision(18, 4);

        builder.HasIndex(e => new { e.TenantId, e.BatchNumber, e.ItemId })
            .IsUnique()
            .HasDatabaseName("idx_batch_tenant_number_item");
    }
}
