namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class GoodsReceivedNote : AuditableEntity
{
    public string GrnNumber { get; set; } = string.Empty;
    public DateOnly GrnDate { get; set; }
    public GrnType GrnType { get; set; }
    public GrnStatus Status { get; set; } = GrnStatus.Draft;
    public Guid SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public Guid? WarehouseId { get; set; }
    public string? WarehouseName { get; set; }
    public Guid? VendorId { get; set; }
    public string? VendorName { get; set; }
    public Guid? CustomerId { get; set; }
    public string? CustomerName { get; set; }
    public string? ReceiptReason { get; set; }
    public string? InternalRefNo { get; set; }
    public Guid? ApprovedBy { get; set; }
    public Guid? ReturnableDcId { get; set; }
    public string? SupplierChallanNo { get; set; }
    public DateOnly? SupplierChallanDate { get; set; }
    public Guid? GateInwardId { get; set; }
    public decimal ExchangeRate { get; set; } = 1;
    public decimal BasicValue { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal AdditionalChargesTotal { get; set; }
    public decimal TotalGrnValue { get; set; }
    public string? Remarks { get; set; }

    // Missing fields identified from form wireframes + KB schema
    public Guid? AdditionalChargesStructId { get; set; }
    public string? CustomerGstin { get; set; }
    public string? ContactPerson { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public string? CreatedByName { get; set; }
    public string? Attachments { get; set; } // JSON array of file metadata
    // Logistics fields (from form wireframes)
    public string? TransporterName { get; set; }
    public string? VehicleNumber { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public string? EwayBillNumber { get; set; }

    public List<GrnLineItem> LineItems { get; set; } = [];
    public GateInward? GateInward { get; set; }
}

public class GrnLineItem : AuditableEntity
{
    public Guid GrnId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public Guid? PurchaseOrderLineId { get; set; }
    public decimal OrderedQuantity { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public decimal AcceptedQuantity { get; set; }
    public decimal RejectedQuantity { get; set; }
    public string Uom { get; set; } = string.Empty;
    public decimal UnitRate { get; set; }
    public decimal BasicAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid? BinId { get; set; }
    public string? BatchNumber { get; set; }
    public string? SerialNumbers { get; set; } // JSON array for serial tracking
    public string? Remarks { get; set; }

    // Missing fields from KB schema
    public Guid? SalesOrderId { get; set; }       // For Customer Supplied
    public bool QcRequired { get; set; }
    public string QcStatus { get; set; } = "NotRequired"; // NotRequired, Pending, Passed, Failed
    public string? ConditionOnReturn { get; set; } // Mandatory for Return Inward: Repaired, Unused, Damaged
    public string? TagNumber { get; set; }
    public decimal TaxRate { get; set; }
    public decimal AdditionalCharges { get; set; }
    // Dimension fields for LBT/Long items
    public decimal? DimLength { get; set; }
    public decimal? DimWidth { get; set; }
    public decimal? DimThickness { get; set; }
    public int? DimPieces { get; set; }
    public decimal? CalculatedWeight { get; set; }

    public GoodsReceivedNote Grn { get; set; } = null!;
}
