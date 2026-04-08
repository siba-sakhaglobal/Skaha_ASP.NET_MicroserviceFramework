namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class SerialNumberConfiguration : IEntityTypeConfiguration<SerialNumber>
{
    public void Configure(EntityTypeBuilder<SerialNumber> builder)
    {
        builder.ToTable("serial_numbers");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.SerialNo).HasColumnName("serial_no").HasMaxLength(100).IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.BatchId).HasColumnName("batch_id");
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.BinId).HasColumnName("bin_id");
        builder.Property(e => e.Status).HasColumnName("status").HasMaxLength(20);
        builder.Property(e => e.CurrentDocType).HasColumnName("current_doc_type").HasMaxLength(50);
        builder.Property(e => e.CurrentDocId).HasColumnName("current_doc_id");

        builder.HasIndex(e => new { e.TenantId, e.SerialNo, e.ItemId })
            .IsUnique()
            .HasDatabaseName("idx_serial_tenant_no_item");
    }
}
