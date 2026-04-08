'use client';

import Link from 'next/link';
import { usePathname } from 'next/navigation';
import { clsx } from 'clsx';
import { INVENTORY_NAV } from '@/lib/constants';

export function Sidebar() {
  const pathname = usePathname();

  const modules = [
    { label: 'Inventory', icon: '📦', items: INVENTORY_NAV, basePath: '/inventory' },
  ];

  return (
    <aside className="fixed left-0 top-[var(--header-height)] bottom-0 w-[var(--sidebar-width)] bg-white border-r border-gray-200 overflow-y-auto z-30">
      <nav className="p-3 space-y-1">
        {modules.map(mod => (
          <div key={mod.label}>
            <div className="px-3 py-2 text-xs font-semibold text-gray-400 uppercase tracking-wider">
              {mod.icon} {mod.label}
            </div>
            {mod.items.map(item => (
              <Link
                key={item.href}
                href={item.href}
                className={clsx(
                  'flex items-center gap-2 px-3 py-2 rounded-lg text-sm transition-colors',
                  pathname === item.href
                    ? 'bg-blue-50 text-[var(--color-primary)] font-medium'
                    : 'text-gray-600 hover:bg-gray-50'
                )}
              >
                {item.label}
              </Link>
            ))}
          </div>
        ))}
      </nav>
    </aside>
  );
}
