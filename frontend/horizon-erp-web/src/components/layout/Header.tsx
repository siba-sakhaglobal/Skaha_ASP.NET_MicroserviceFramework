'use client';

import { useAuth } from '@/contexts/AuthContext';
import { useTenant } from '@/contexts/TenantContext';

export function Header() {
  const { user, logout } = useAuth();
  const { tenant } = useTenant();

  return (
    <header className="fixed top-0 left-0 right-0 h-[var(--header-height)] bg-white border-b border-gray-200 flex items-center justify-between px-4 z-40">
      <div className="flex items-center gap-3">
        <h1 className="text-lg font-bold text-[var(--color-primary)]">
          {tenant?.tenantName || 'Horizon Premier'}
        </h1>
      </div>

      <div className="flex items-center gap-2">
        <input
          type="search"
          placeholder="Search..."
          className="w-64 px-3 py-1.5 text-sm border border-gray-300 rounded-lg focus:outline-none focus:ring-2 focus:ring-[var(--color-primary)]"
        />
        <div className="flex items-center gap-2 ml-4">
          <div className="text-right">
            <p className="text-sm font-medium text-gray-700">{user?.name || 'Dev User'}</p>
            <p className="text-xs text-gray-500">{user?.role || 'Admin'}</p>
          </div>
          <button onClick={logout} className="text-sm text-gray-500 hover:text-red-500 ml-2">Logout</button>
        </div>
      </div>
    </header>
  );
}
