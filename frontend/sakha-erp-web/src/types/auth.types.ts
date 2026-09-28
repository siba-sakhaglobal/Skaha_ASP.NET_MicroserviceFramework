export interface AuthState {
  isAuthenticated: boolean;
  user: import('./common.types').UserInfo | null;
  token: string | null;
  refreshToken: string | null;
  isLoading: boolean;
}

export interface LoginRequest {
  email: string;
  password: string;
  tenantId?: string;
}

export interface LoginResponse {
  token: string;
  refreshToken: string;
  user: import('./common.types').UserInfo;
  tenantConfig: import('./common.types').TenantConfig;
}
