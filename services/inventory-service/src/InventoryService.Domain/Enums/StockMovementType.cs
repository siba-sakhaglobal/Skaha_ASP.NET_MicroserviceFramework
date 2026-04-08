namespace InventoryService.Domain.Enums;

public enum StockMovementType
{
    OpeningStock,
    GRN,
    StockTransferOut,
    StockTransferIn,
    AdjustmentIncrease,
    AdjustmentDecrease,
    IssueToShopFloor,
    ReturnFromShopFloor,
    ProductionReceipt,
    SalesDispatch,
    SalesReturn,
    ScrapReceipt,
    SubContractSendOut,
    SubContractReceipt
}
