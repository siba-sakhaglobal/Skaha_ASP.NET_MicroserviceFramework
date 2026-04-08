'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useTenant } from '@/contexts/TenantContext';

export function usePermission() {
  const { hasPermission } = useAuth();
  const { isModuleEnabled } = useTenant();

  const canAccess = (module: string, action?: string) => {
    if (!isModuleEnabled(module)) return false;
    if (action) return hasPermission(`${module}.${action}`);
    return true;
  };

  return { canAccess, hasPermission, isModuleEnabled };
}
