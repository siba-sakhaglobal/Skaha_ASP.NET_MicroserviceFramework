namespace InventoryService.Application.GRN.Commands;

using InventoryService.Application.GRN.DTOs;
using InventoryService.Application.Common.Interfaces;
using InventoryService.Domain.Entities;
using InventoryService.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

public class CreateGrnHandler : IRequestHandler<CreateGrnCommand, GrnDetailDto>
{
    private readonly IInventoryDbContext _db;

    public CreateGrnHandler(IInventoryDbContext db) { _db = db; }

    public async Task<GrnDetailDto> Handle(CreateGrnCommand command, CancellationToken ct)
    {
        var req = command.Request;

        // Generate GRN number: GRN-{year}-{month}-{sequence:D4}
        var now = DateTime.UtcNow;
        var prefix = $"GRN-{now:yyyy}-{now:MM}-";
        var count = await _db.GoodsReceivedNotes
            .CountAsync(g => g.GrnNumber.StartsWith(prefix), ct);
        var grnNumber = $"{prefix}{(count + 1):D4}";

        // Parse enums
        var grnType = Enum.Parse<GrnType>(req.GrnType, true);
        DateOnly.TryParse(req.GrnDate, out var grnDate);
        DateOnly? challanDate = req.SupplierChallanDate is not null
            ? DateOnly.Parse(req.SupplierChallanDate) : null;

        // Map line items
        var lineItems = req.LineItems.Select(li =>
        {
            var basicAmount = li.ReceivedQuantity * li.UnitRate;
            var taxAmount = basicAmount * li.TaxRate / 100m;
            var totalAmount = basicAmount + taxAmount;

            return new GrnLineItem
            {
                ItemId = li.ItemId,
                ItemCode = li.ItemCode,
                ItemName = li.ItemName,
                SalesOrderId = li.SalesOrderId,
                OrderedQuantity = 0,
                ReceivedQuantity = li.ReceivedQuantity,
                AcceptedQuantity = li.ReceivedQuantity,
                RejectedQuantity = 0,
                Uom = li.Uom,
                UnitRate = li.UnitRate,
                TaxRate = li.TaxRate,
                BasicAmount = basicAmount,
                TaxAmount = taxAmount,
                TotalAmount = totalAmount,
                AdditionalCharges = 0,
                WarehouseId = li.WarehouseId,
                BinId = li.BinId,
                BatchNumber = li.BatchNumber,
                QcStatus = li.QcStatus,
                ConditionOnReturn = li.ConditionOnReturn,
                Remarks = li.Remarks
            };
        }).ToList();

        // Calculate header totals
        var basicValue = lineItems.Sum(l => l.BasicAmount);
        var taxAmount = lineItems.Sum(l => l.TaxAmount);
        var additionalChargesTotal = lineItems.Sum(l => l.AdditionalCharges);
        var totalGrnValue = basicValue + taxAmount + additionalChargesTotal;

        var grn = new GoodsReceivedNote
        {
            GrnNumber = grnNumber,
            GrnDate = grnDate,
            GrnType = grnType,
            Status = GrnStatus.Draft,
            SiteId = req.SiteId,
            SiteName = req.SiteName,
            WarehouseId = req.WarehouseId,
            WarehouseName = req.WarehouseName,
            VendorId = req.VendorId,
            VendorName = req.VendorName,
            CustomerId = req.CustomerId,
            CustomerName = req.CustomerName,
            CustomerGstin = req.CustomerGstin,
            ContactPerson = req.ContactPerson,
            ReceiptReason = req.ReceiptReason,
            SupplierChallanNo = req.SupplierChallanNo,
            SupplierChallanDate = challanDate,
            GateInwardId = req.GateInwardId,
            TransporterName = req.TransporterName,
            VehicleNumber = req.VehicleNumber,
            DriverName = req.DriverName,
            DriverContact = req.DriverContact,
            EwayBillNumber = req.EwayBillNumber,
            ExchangeRate = 1,
            BasicValue = basicValue,
            TaxAmount = taxAmount,
            AdditionalChargesTotal = additionalChargesTotal,
            TotalGrnValue = totalGrnValue,
            Remarks = req.Remarks,
            Attachments = req.Attachments,
            LineItems = lineItems
        };

        _db.GoodsReceivedNotes.Add(grn);
        await _db.SaveChangesAsync(ct);

        return MapToDetailDto(grn);
    }

    internal static GrnDetailDto MapToDetailDto(GoodsReceivedNote g) => new(
        g.Id,
        g.GrnNumber,
        g.GrnDate.ToString("yyyy-MM-dd"),
        g.GrnType.ToString(),
        g.Status.ToString(),
        g.SiteId,
        g.SiteName,
        g.WarehouseId,
        g.WarehouseName,
        g.VendorId,
        g.VendorName,
        g.CustomerId,
        g.CustomerName,
        g.CustomerGstin,
        g.ContactPerson,
        g.ReceiptReason,
        g.SupplierChallanNo,
        g.SupplierChallanDate?.ToString("yyyy-MM-dd"),
        g.GateInwardId,
        g.TransporterName,
        g.VehicleNumber,
        g.DriverName,
        g.DriverContact,
        g.EwayBillNumber,
        g.ExchangeRate,
        g.BasicValue,
        g.TaxAmount,
        g.AdditionalChargesTotal,
        g.TotalGrnValue,
        g.Remarks,
        g.Attachments,
        g.CreatedByName,
        g.CreatedAt.ToString("yyyy-MM-ddTHH:mm:ssZ"),
        g.LineItems.Select(MapToLineItemDto).ToList());

    internal static GrnLineItemDto MapToLineItemDto(GrnLineItem l) => new(
        l.Id,
        l.ItemId,
        l.ItemCode,
        l.ItemName,
        l.SalesOrderId,
        l.OrderedQuantity,
        l.ReceivedQuantity,
        l.AcceptedQuantity,
        l.RejectedQuantity,
        l.Uom,
        l.UnitRate,
        l.TaxRate,
        l.BasicAmount,
        l.TaxAmount,
        l.AdditionalCharges,
        l.TotalAmount,
        l.QcStatus,
        l.ConditionOnReturn,
        l.Remarks);
}
