namespace InventoryService.Application.GRN.DTOs;

public record GrnListDto(
    Guid Id, string GrnNumber, string GrnDate, string GrnType, string Status,
    string SiteName, string? WarehouseName, string? VendorName, string? CustomerName,
    decimal TotalGrnValue, int LineItemCount, string CreatedAt);

public record GrnDetailDto(
    Guid Id, string GrnNumber, string GrnDate, string GrnType, string Status,
    Guid SiteId, string SiteName, Guid? WarehouseId, string? WarehouseName,
    Guid? VendorId, string? VendorName, Guid? CustomerId, string? CustomerName,
    string? CustomerGstin, string? ContactPerson, string? ReceiptReason,
    string? SupplierChallanNo, string? SupplierChallanDate, Guid? GateInwardId,
    string? TransporterName, string? VehicleNumber, string? DriverName, string? DriverContact,
    string? EwayBillNumber, decimal ExchangeRate, decimal BasicValue, decimal TaxAmount,
    decimal AdditionalChargesTotal, decimal TotalGrnValue, string? Remarks, string? Attachments,
    string? CreatedByName, string CreatedAt,
    List<GrnLineItemDto> LineItems);

public record GrnLineItemDto(
    Guid Id, Guid ItemId, string ItemCode, string ItemName, Guid? SalesOrderId,
    decimal OrderedQuantity, decimal ReceivedQuantity, decimal AcceptedQuantity, decimal RejectedQuantity,
    string Uom, decimal UnitRate, decimal TaxRate, decimal BasicAmount, decimal TaxAmount,
    decimal AdditionalCharges, decimal TotalAmount,
    string QcStatus, string? ConditionOnReturn, string? Remarks);

public record CreateGrnRequest(
    string GrnDate, string GrnType, Guid SiteId, string SiteName,
    Guid? WarehouseId, string? WarehouseName,
    Guid? VendorId, string? VendorName, Guid? CustomerId, string? CustomerName,
    string? CustomerGstin, string? ContactPerson, string? ReceiptReason,
    string? SupplierChallanNo, string? SupplierChallanDate, Guid? GateInwardId,
    string? TransporterName, string? VehicleNumber, string? DriverName, string? DriverContact,
    string? EwayBillNumber, string? Remarks, string? Attachments,
    List<CreateGrnLineItemRequest> LineItems);

public record CreateGrnLineItemRequest(
    Guid ItemId, string ItemCode, string ItemName, Guid? SalesOrderId,
    decimal ReceivedQuantity, string Uom, decimal UnitRate, decimal TaxRate,
    Guid WarehouseId, Guid? BinId, string? BatchNumber,
    string QcStatus, string? ConditionOnReturn, string? Remarks);
