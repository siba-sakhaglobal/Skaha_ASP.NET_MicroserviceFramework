namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class GateInward : AuditableEntity
{
    public string GateInwardNo { get; set; } = string.Empty;
    public DateOnly GateInwardDate { get; set; }
    public TimeOnly GateInwardTime { get; set; }
    public GateInwardStatus Status { get; set; } = GateInwardStatus.Draft;
    public Guid SiteId { get; set; }
    public string SiteName { get; set; } = string.Empty;
    public string ReferenceType { get; set; } = string.Empty; // PO, StockTransfer, CustomerReturn, DC
    public Guid ReferenceDocId { get; set; }
    public string ReferenceDocNo { get; set; } = string.Empty;
    public Guid? VendorCustomerId { get; set; }
    public string? VendorCustomerName { get; set; }
    public string? Gstin { get; set; }
    public string? TransporterName { get; set; }
    public string VehicleNumber { get; set; } = string.Empty;
    public int? NumberOfPieces { get; set; }
    public decimal? InwardValue { get; set; }
    public string? Remarks { get; set; }
    public string? DriverName { get; set; }
    public string? DriverContact { get; set; }
    public string? LrNumber { get; set; }        // Lorry Receipt
    public DateOnly? LrDate { get; set; }
    public string? EwayBillNumber { get; set; }
    public string? Attachments { get; set; }      // JSON array

    public List<GateInwardItem> Items { get; set; } = [];
}

public class GateInwardItem : AuditableEntity
{
    public Guid GateInwardId { get; set; }
    public Guid ItemId { get; set; }
    public string ItemCode { get; set; } = string.Empty;
    public string ItemName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Uom { get; set; } = string.Empty;
    public string? Remarks { get; set; }

    public GateInward GateInward { get; set; } = null!;
}
