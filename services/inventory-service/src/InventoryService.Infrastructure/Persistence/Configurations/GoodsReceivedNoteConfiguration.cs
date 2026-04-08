namespace InventoryService.Infrastructure.Persistence.Configurations;

using InventoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class GoodsReceivedNoteConfiguration : IEntityTypeConfiguration<GoodsReceivedNote>
{
    public void Configure(EntityTypeBuilder<GoodsReceivedNote> builder)
    {
        builder.ToTable("goods_received_notes");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();

        builder.Property(e => e.GrnNumber).HasColumnName("grn_number").HasMaxLength(50).IsRequired();
        builder.Property(e => e.GrnDate).HasColumnName("grn_date").IsRequired();
        builder.Property(e => e.GrnType).HasColumnName("grn_type").HasConversion<string>().HasMaxLength(30).IsRequired();
        builder.Property(e => e.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(30).IsRequired();

        builder.Property(e => e.SiteId).HasColumnName("site_id").IsRequired();
        builder.Property(e => e.SiteName).HasColumnName("site_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id");
        builder.Property(e => e.WarehouseName).HasColumnName("warehouse_name").HasMaxLength(200);

        builder.Property(e => e.VendorId).HasColumnName("vendor_id");
        builder.Property(e => e.VendorName).HasColumnName("vendor_name").HasMaxLength(200);
        builder.Property(e => e.CustomerId).HasColumnName("customer_id");
        builder.Property(e => e.CustomerName).HasColumnName("customer_name").HasMaxLength(200);
        builder.Property(e => e.CustomerGstin).HasColumnName("customer_gstin").HasMaxLength(20);
        builder.Property(e => e.ContactPerson).HasColumnName("contact_person").HasMaxLength(150);
        builder.Property(e => e.ContactEmail).HasColumnName("contact_email").HasMaxLength(200);
        builder.Property(e => e.ContactPhone).HasColumnName("contact_phone").HasMaxLength(30);

        builder.Property(e => e.ReceiptReason).HasColumnName("receipt_reason").HasMaxLength(200);
        builder.Property(e => e.InternalRefNo).HasColumnName("internal_ref_no").HasMaxLength(50);
        builder.Property(e => e.ApprovedBy).HasColumnName("approved_by");
        builder.Property(e => e.ReturnableDcId).HasColumnName("returnable_dc_id");
        builder.Property(e => e.SupplierChallanNo).HasColumnName("supplier_challan_no").HasMaxLength(50);
        builder.Property(e => e.SupplierChallanDate).HasColumnName("supplier_challan_date");
        builder.Property(e => e.GateInwardId).HasColumnName("gate_inward_id");
        builder.Property(e => e.AdditionalChargesStructId).HasColumnName("additional_charges_struct_id");

        // Logistics
        builder.Property(e => e.TransporterName).HasColumnName("transporter_name").HasMaxLength(200);
        builder.Property(e => e.VehicleNumber).HasColumnName("vehicle_number").HasMaxLength(30);
        builder.Property(e => e.DriverName).HasColumnName("driver_name").HasMaxLength(150);
        builder.Property(e => e.DriverContact).HasColumnName("driver_contact").HasMaxLength(30);
        builder.Property(e => e.EwayBillNumber).HasColumnName("eway_bill_number").HasMaxLength(20);

        // Financials
        builder.Property(e => e.ExchangeRate).HasColumnName("exchange_rate").HasPrecision(18, 6);
        builder.Property(e => e.BasicValue).HasColumnName("basic_value").HasPrecision(18, 4);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 4);
        builder.Property(e => e.AdditionalChargesTotal).HasColumnName("additional_charges_total").HasPrecision(18, 4);
        builder.Property(e => e.TotalGrnValue).HasColumnName("total_grn_value").HasPrecision(18, 4);

        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);
        builder.Property(e => e.Attachments).HasColumnName("attachments"); // JSON
        builder.Property(e => e.CreatedByName).HasColumnName("created_by_name").HasMaxLength(200);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Indexes
        builder.HasIndex(e => e.GrnNumber).HasDatabaseName("ix_goods_received_notes_grn_number");
        builder.HasIndex(e => new { e.TenantId, e.SiteId }).HasDatabaseName("ix_goods_received_notes_tenant_site");
        builder.HasIndex(e => new { e.TenantId, e.GrnType, e.Status }).HasDatabaseName("ix_goods_received_notes_tenant_type_status");
        builder.HasIndex(e => e.GrnDate).HasDatabaseName("ix_goods_received_notes_grn_date");
        builder.HasIndex(e => e.VendorId).HasDatabaseName("ix_goods_received_notes_vendor_id");
        builder.HasIndex(e => e.CustomerId).HasDatabaseName("ix_goods_received_notes_customer_id");
        builder.HasIndex(e => e.GateInwardId).HasDatabaseName("ix_goods_received_notes_gate_inward_id");

        // Relationships
        builder.HasMany(e => e.LineItems)
            .WithOne(e => e.Grn)
            .HasForeignKey(e => e.GrnId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(e => e.GateInward)
            .WithMany()
            .HasForeignKey(e => e.GateInwardId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}

public class GrnLineItemConfiguration : IEntityTypeConfiguration<GrnLineItem>
{
    public void Configure(EntityTypeBuilder<GrnLineItem> builder)
    {
        builder.ToTable("grn_line_items");

        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.TenantId).HasColumnName("tenant_id").IsRequired();
        builder.Property(e => e.GrnId).HasColumnName("grn_id").IsRequired();

        builder.Property(e => e.ItemId).HasColumnName("item_id").IsRequired();
        builder.Property(e => e.ItemCode).HasColumnName("item_code").HasMaxLength(50).IsRequired();
        builder.Property(e => e.ItemName).HasColumnName("item_name").HasMaxLength(200).IsRequired();
        builder.Property(e => e.PurchaseOrderLineId).HasColumnName("purchase_order_line_id");
        builder.Property(e => e.SalesOrderId).HasColumnName("sales_order_id");

        builder.Property(e => e.OrderedQuantity).HasColumnName("ordered_quantity").HasPrecision(18, 4);
        builder.Property(e => e.ReceivedQuantity).HasColumnName("received_quantity").HasPrecision(18, 4);
        builder.Property(e => e.AcceptedQuantity).HasColumnName("accepted_quantity").HasPrecision(18, 4);
        builder.Property(e => e.RejectedQuantity).HasColumnName("rejected_quantity").HasPrecision(18, 4);
        builder.Property(e => e.Uom).HasColumnName("uom").HasMaxLength(20).IsRequired();
        builder.Property(e => e.UnitRate).HasColumnName("unit_rate").HasPrecision(18, 4);
        builder.Property(e => e.TaxRate).HasColumnName("tax_rate").HasPrecision(6, 2);
        builder.Property(e => e.BasicAmount).HasColumnName("basic_amount").HasPrecision(18, 4);
        builder.Property(e => e.TaxAmount).HasColumnName("tax_amount").HasPrecision(18, 4);
        builder.Property(e => e.AdditionalCharges).HasColumnName("additional_charges").HasPrecision(18, 4);
        builder.Property(e => e.TotalAmount).HasColumnName("total_amount").HasPrecision(18, 4);

        builder.Property(e => e.WarehouseId).HasColumnName("warehouse_id").IsRequired();
        builder.Property(e => e.BinId).HasColumnName("bin_id");
        builder.Property(e => e.BatchNumber).HasColumnName("batch_number").HasMaxLength(50);
        builder.Property(e => e.SerialNumbers).HasColumnName("serial_numbers"); // JSON
        builder.Property(e => e.TagNumber).HasColumnName("tag_number").HasMaxLength(50);

        // QC fields
        builder.Property(e => e.QcRequired).HasColumnName("qc_required").HasDefaultValue(false);
        builder.Property(e => e.QcStatus).HasColumnName("qc_status").HasMaxLength(20).IsRequired();
        builder.Property(e => e.ConditionOnReturn).HasColumnName("condition_on_return").HasMaxLength(50);

        // Dimension fields
        builder.Property(e => e.DimLength).HasColumnName("dim_length").HasPrecision(18, 4);
        builder.Property(e => e.DimWidth).HasColumnName("dim_width").HasPrecision(18, 4);
        builder.Property(e => e.DimThickness).HasColumnName("dim_thickness").HasPrecision(18, 4);
        builder.Property(e => e.DimPieces).HasColumnName("dim_pieces");
        builder.Property(e => e.CalculatedWeight).HasColumnName("calculated_weight").HasPrecision(18, 4);

        builder.Property(e => e.Remarks).HasColumnName("remarks").HasMaxLength(500);

        // Audit
        builder.Property(e => e.CreatedAt).HasColumnName("created_at").IsRequired();
        builder.Property(e => e.CreatedBy).HasColumnName("created_by").IsRequired();
        builder.Property(e => e.ModifiedAt).HasColumnName("modified_at");
        builder.Property(e => e.ModifiedBy).HasColumnName("modified_by");
        builder.Property(e => e.IsDeleted).HasColumnName("is_deleted").HasDefaultValue(false);

        // Indexes
        builder.HasIndex(e => e.GrnId).HasDatabaseName("ix_grn_line_items_grn_id");
        builder.HasIndex(e => e.ItemId).HasDatabaseName("ix_grn_line_items_item_id");
        builder.HasIndex(e => e.SalesOrderId).HasDatabaseName("ix_grn_line_items_sales_order_id");
        builder.HasIndex(e => e.WarehouseId).HasDatabaseName("ix_grn_line_items_warehouse_id");

        builder.HasQueryFilter(e => !e.IsDeleted);
    }
}
