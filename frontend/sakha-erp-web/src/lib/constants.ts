export const STATUS_COLORS: Record<string, string> = {
  Draft: 'bg-gray-100 text-gray-700',
  Active: 'bg-green-100 text-green-700',
  Inactive: 'bg-gray-100 text-gray-500',
  Posted: 'bg-blue-100 text-blue-700',
  Approved: 'bg-blue-100 text-blue-700',
  Received: 'bg-green-100 text-green-700',
  UnderQC: 'bg-yellow-100 text-yellow-700',
  Accepted: 'bg-green-100 text-green-700',
  PartiallyRejected: 'bg-orange-100 text-orange-700',
  Completed: 'bg-green-100 text-green-700',
  Cancelled: 'bg-red-100 text-red-700',
  Dispatched: 'bg-purple-100 text-purple-700',
  InTransit: 'bg-indigo-100 text-indigo-700',
  PendingApproval: 'bg-yellow-100 text-yellow-700',
};

export const INVENTORY_NAV = [
  { label: 'Overview', href: '/inventory', icon: 'LayoutDashboard' },
  { label: 'Item-Warehouse Mappings', href: '/inventory/item-warehouse-mappings', icon: 'MapPin' },
  { label: 'Gate Inward', href: '/inventory/gate-inwards', icon: 'LogIn' },
  { label: 'Goods Receipt (GRN)', href: '/inventory/grn', icon: 'PackageCheck' },
  { label: 'Stock Levels', href: '/inventory/stock-levels', icon: 'BarChart3' },
  { label: 'Stock Transfers', href: '/inventory/stock-transfers', icon: 'ArrowLeftRight' },
  { label: 'Stock Adjustments', href: '/inventory/stock-adjustments', icon: 'SlidersHorizontal' },
];
