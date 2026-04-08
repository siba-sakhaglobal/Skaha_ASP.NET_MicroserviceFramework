import api from './api';
import type { ApiResponse, PagedResponse, PagedRequest } from '@/types/api.types';
import type {
  ItemWarehouseMapping,
  CreateItemWarehouseMappingRequest,
  GateInward,
  GoodsReceivedNote,
  StockBalance,
} from '@/types/inventory.types';

const BASE = '/api/v1/inventory';

export const inventoryApi = {
  // Item-Warehouse Mappings
  itemWarehouseMappings: {
    list: (params?: PagedRequest & { itemId?: string; warehouseId?: string; status?: string }) =>
      api.get<ApiResponse<PagedResponse<ItemWarehouseMapping>>>(`${BASE}/item-warehouse-mappings`, { params }),
    getById: (id: string) =>
      api.get<ApiResponse<ItemWarehouseMapping>>(`${BASE}/item-warehouse-mappings/${id}`),
    create: (data: CreateItemWarehouseMappingRequest) =>
      api.post<ApiResponse<ItemWarehouseMapping>>(`${BASE}/item-warehouse-mappings`, data),
    update: (id: string, data: Partial<ItemWarehouseMapping>) =>
      api.put<ApiResponse<ItemWarehouseMapping>>(`${BASE}/item-warehouse-mappings/${id}`, data),
    delete: (id: string) =>
      api.delete(`${BASE}/item-warehouse-mappings/${id}`),
  },

  // Gate Inwards
  gateInwards: {
    list: (params?: PagedRequest & { status?: string; siteId?: string }) =>
      api.get<ApiResponse<PagedResponse<GateInward>>>(`${BASE}/gate-inwards`, { params }),
    getById: (id: string) =>
      api.get<ApiResponse<GateInward>>(`${BASE}/gate-inwards/${id}`),
    create: (data: Partial<GateInward>) =>
      api.post<ApiResponse<GateInward>>(`${BASE}/gate-inwards`, data),
    post: (id: string) =>
      api.post<ApiResponse<GateInward>>(`${BASE}/gate-inwards/${id}/post`),
    cancel: (id: string) =>
      api.post<ApiResponse<GateInward>>(`${BASE}/gate-inwards/${id}/cancel`),
  },

  // GRN
  grn: {
    list: (params?: PagedRequest & { grnType?: string; status?: string }) =>
      api.get<ApiResponse<PagedResponse<GoodsReceivedNote>>>(`${BASE}/grns`, { params }),
    getById: (id: string) =>
      api.get<ApiResponse<GoodsReceivedNote>>(`${BASE}/grns/${id}`),
    create: (data: Partial<GoodsReceivedNote>) =>
      api.post<ApiResponse<GoodsReceivedNote>>(`${BASE}/grns`, data),
    post: (id: string) =>
      api.post<ApiResponse<GoodsReceivedNote>>(`${BASE}/grns/${id}/post`),
    cancel: (id: string) =>
      api.post<ApiResponse<GoodsReceivedNote>>(`${BASE}/grns/${id}/cancel`),
  },

  // Stock Levels
  stockLevels: {
    current: (params?: { itemId?: string; warehouseId?: string }) =>
      api.get<ApiResponse<StockBalance[]>>(`${BASE}/stock-levels/current`, { params }),
    ledger: (params?: { itemId?: string; dateFrom?: string; dateTo?: string }) =>
      api.get<ApiResponse<any[]>>(`${BASE}/stock-levels/ledger`, { params }),
    valuation: (params?: { warehouseId?: string }) =>
      api.get<ApiResponse<any[]>>(`${BASE}/stock-levels/valuation`, { params }),
  },
};
