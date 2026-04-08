namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class InventorySettingsConfiguration : IEntityTypeConfiguration<InventorySettings>
{
    public void Configure(EntityTypeBuilder<InventorySettings> builder)
    {
        builder.ToTable("inventory_settings");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.AllowNegativeStock).HasColumnName("allow_negative_stock");
        builder.Property(e => e.GateInwardMandatory).HasColumnName("gate_inward_mandatory");
        builder.Property(e => e.QcMandatoryOnGrn).HasColumnName("qc_mandatory_on_grn");
        builder.Property(e => e.DefaultValuationMethod).HasColumnName("default_valuation_method").HasConversion<string>().HasMaxLength(30);
        builder.Property(e => e.AutoBatchGeneration).HasColumnName("auto_batch_generation");
        builder.Property(e => e.BatchNumberFormat).HasColumnName("batch_number_format").HasMaxLength(100);
        builder.Property(e => e.AutoSerialGeneration).HasColumnName("auto_serial_generation");
        builder.Property(e => e.SerialNumberFormat).HasColumnName("serial_number_format").HasMaxLength(100);

        builder.HasIndex(e => e.TenantId).IsUnique().HasDatabaseName("idx_settings_tenant");
    }
}
