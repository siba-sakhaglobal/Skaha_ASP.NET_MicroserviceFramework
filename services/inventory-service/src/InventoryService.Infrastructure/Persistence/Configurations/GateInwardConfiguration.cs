namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GateInwardConfiguration : IEntityTypeConfiguration<GateInward>
{
    public void Configure(EntityTypeBuilder<GateInward> builder)
    {
        builder.ToTable("gate_inwards");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.GateInwardNo).HasColumnName("gate_inward_no").HasMaxLength(50).IsRequired();
        builder.Property(e => e.GateInwardDate).HasColumnName("gate_inward_date").IsRequired();
        builder.Property(e => e.GateInwardTime).HasColumnName("gate_inward_time").IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20).IsRequired();
        builder.Property(e => e.SiteId).HasColumnName("site_id").IsRequired();
        builder.Property(e => e.SiteName).HasColumnName("site_name").HasMaxLength(200);
        builder.Property(e => e.ReferenceType).HasColumnName("reference_type").HasMaxLength(50);
        builder.Property(e => e.ReferenceDocId).HasColumnName("reference_doc_id");
        builder.Property(e => e.ReferenceDocNo).HasColumnName("reference_doc_no").HasMaxLength(50);
        builder.Property(e => e.VendorCustomerId).HasColumnName("vendor_customer_id");
        builder.Property(e => e.VendorCustomerName).HasColumnName("vendor_customer_name").HasMaxLength(200);
        builder.Property(e => e.Gstin).HasColumnName("gstin").HasMaxLength(15);
        builder.Property(e => e.TransporterName).HasColumnName("transporter_name").HasMaxLength(200);
        builder.Property(e => e.VehicleNumber).HasColumnName("vehicle_number").HasMaxLength(20);
        builder.Property(e => e.NumberOfPieces).HasColumnName("number_of_pieces");
        builder.Property(e => e.InwardValue).HasColumnName("inward_value").HasPrecision(18, 4);
        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasMany(e => e.Items).WithOne(e => e.GateInward).HasForeignKey(e => e.GateInwardId);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class GateInwardItemConfiguration : IEntityTypeConfiguration<GateInwardItem>
{
    public void Configure(EntityTypeBuilder<GateInwardItem> builder)
    {
        builder.ToTable("gate_inward_items");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.GateInwardId).HasColumnName("gate_inward_id").IsRequired();
        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50);
        builder.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(200);
        builder.Property(e => e.Quantity).HasColumnName("quantity").HasPrecision(18, 4);
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20);
        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
