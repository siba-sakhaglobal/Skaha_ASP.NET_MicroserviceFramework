export interface ApiResponse<T> {
  success: boolean;
  data: T | null;
  errors?: ApiError[];
}

export interface ApiError {
  type: string;
  title: string;
  status: number;
  detail: string;
  field?: string;
}

export interface PagedResponse<T> {
  items: T[];
  page: number;
  pageSize: number;
  totalItems: number;
  totalPages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

export interface PagedRequest {
  page?: number;
  pageSize?: number;
  sortBy?: string;
  sortOrder?: 'asc' | 'desc';
  search?: string;
}
