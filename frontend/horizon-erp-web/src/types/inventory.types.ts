export interface ItemWarehouseMapping {
  id: string;
  mappingCode: string;
  itemId: string;
  itemCode: string;
  itemName: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  warehouseBinId?: string;
  binName?: string;
  status: 'Active' | 'Inactive';
  traceabilityLevel: 'None' | 'Batch' | 'Serial';
  openingQuantity?: number;
  openingRate?: number;
  openingStockValue?: number;
  openingStockDate?: string;
  minQuantity?: number;
  reorderQuantity?: number;
  maxQuantity?: number;
  createdAt: string;
}

export interface CreateItemWarehouseMappingRequest {
  itemId: string;
  itemCode: string;
  itemName: string;
  warehouseId: string;
  warehouseCode: string;
  warehouseName: string;
  warehouseBinId?: string;
  binName?: string;
  traceabilityLevel?: string;
  openingQuantity?: number;
  openingRate?: number;
  openingStockDate?: string;
  minQuantity?: number;
  reorderQuantity?: number;
  maxQuantity?: number;
}

export interface GateInward {
  id: string;
  gateInwardNo: string;
  gateInwardDate: string;
  gateInwardTime: string;
  status: 'Draft' | 'Posted' | 'GrnCreated' | 'Cancelled';
  siteId: string;
  siteName: string;
  referenceType: string;
  referenceDocId: string;
  referenceDocNo: string;
  vendorCustomerName?: string;
  vehicleNumber: string;
  transporterName?: string;
  remarks?: string;
}

export interface GoodsReceivedNote {
  id: string;
  grnNumber: string;
  grnDate: string;
  grnType: 'AgainstPO' | 'WithoutPO' | 'CustomerSupplied' | 'ReturnInward';
  status: 'Draft' | 'Received' | 'UnderQC' | 'Accepted' | 'PartiallyRejected' | 'Completed' | 'Cancelled';
  siteName: string;
  warehouseName?: string;
  vendorName?: string;
  customerName?: string;
  totalGrnValue: number;
  lineItems: GrnLineItem[];
}

export interface GrnLineItem {
  id: string;
  itemCode: string;
  itemName: string;
  orderedQuantity: number;
  receivedQuantity: number;
  acceptedQuantity: number;
  rejectedQuantity: number;
  uom: string;
  unitRate: number;
  totalAmount: number;
}

export interface StockBalance {
  itemId: string;
  itemCode: string;
  warehouseId: string;
  onHandQuantity: number;
  reservedQuantity: number;
  availableQuantity: number;
  averageCost: number;
  totalValue: number;
  uom: string;
}
