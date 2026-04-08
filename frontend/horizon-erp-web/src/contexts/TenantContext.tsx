'use client';

import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import type { TenantConfig } from '@/types/common.types';

interface TenantContextType {
  tenant: TenantConfig | null;
  setTenant: (config: TenantConfig) => void;
  isModuleEnabled: (module: string) => boolean;
  isFeatureEnabled: (feature: string) => boolean;
}

const TenantContext = createContext<TenantContextType | null>(null);

export function TenantProvider({ children }: { children: ReactNode }) {
  const [tenant, setTenantState] = useState<TenantConfig | null>(null);

  useEffect(() => {
    const saved = localStorage.getItem('tenantConfig');
    if (saved) setTenantState(JSON.parse(saved));
  }, []);

  const setTenant = (config: TenantConfig) => {
    localStorage.setItem('tenantConfig', JSON.stringify(config));
    localStorage.setItem('tenantId', config.tenantId);
    setTenantState(config);
    // Apply theme
    document.documentElement.setAttribute('data-theme', config.theme.template);
    document.documentElement.style.setProperty('--color-primary', config.theme.primaryColor);
  };

  const isModuleEnabled = (module: string) =>
    tenant?.enabledModules?.includes(module) ?? false;

  const isFeatureEnabled = (feature: string) =>
    tenant?.features?.[feature] ?? false;

  return (
    <TenantContext.Provider value={{ tenant, setTenant, isModuleEnabled, isFeatureEnabled }}>
      {children}
    </TenantContext.Provider>
  );
}

export const useTenant = () => {
  const ctx = useContext(TenantContext);
  if (!ctx) throw new Error('useTenant must be used within TenantProvider');
  return ctx;
};
