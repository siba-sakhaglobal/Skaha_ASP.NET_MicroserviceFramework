'use client';

import { useState, useCallback } from 'react';
import type { AxiosResponse } from 'axios';
import type { ApiResponse } from '@/types/api.types';

interface UseApiState<T> {
  data: T | null;
  isLoading: boolean;
  error: string | null;
}

export function useApi<T>() {
  const [state, setState] = useState<UseApiState<T>>({ data: null, isLoading: false, error: null });

  const execute = useCallback(async (apiCall: () => Promise<AxiosResponse<ApiResponse<T>>>) => {
    setState({ data: null, isLoading: true, error: null });
    try {
      const response = await apiCall();
      if (response.data.success) {
        setState({ data: response.data.data, isLoading: false, error: null });
        return response.data.data;
      } else {
        const errMsg = response.data.errors?.[0]?.detail || 'An error occurred';
        setState({ data: null, isLoading: false, error: errMsg });
        return null;
      }
    } catch (err: any) {
      const errMsg = err.response?.data?.errors?.[0]?.detail || err.message || 'Network error';
      setState({ data: null, isLoading: false, error: errMsg });
      return null;
    }
  }, []);

  return { ...state, execute };
}
