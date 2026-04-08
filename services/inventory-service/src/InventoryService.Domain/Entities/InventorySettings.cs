namespace InventoryService.Domain.Entities;

using InventoryService.Domain.Enums;

public class InventorySettings
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; } // Unique per tenant
    public bool AllowNegativeStock { get; set; }
    public bool GateInwardMandatory { get; set; }
    public bool QcMandatoryOnGrn { get; set; }
    public ValuationMethod DefaultValuationMethod { get; set; } = ValuationMethod.MovingAverage;
    public bool AutoBatchGeneration { get; set; }
    public string? BatchNumberFormat { get; set; }
    public bool AutoSerialGeneration { get; set; }
    public string? SerialNumberFormat { get; set; }
}
