import { PageHeader } from '@/components/layout/PageHeader';

export default function InventoryPage() {
  const kpis = [
    { label: 'Total Items', value: '1,247', change: '+12', color: 'blue' },
    { label: 'Warehouses', value: '8', change: '', color: 'green' },
    { label: 'Low Stock Alerts', value: '23', change: '+5', color: 'red' },
    { label: 'Pending GRN', value: '15', change: '-3', color: 'yellow' },
  ];

  return (
    <>
      <PageHeader
        title="Inventory"
        subtitle="Stock management across all warehouses"
        breadcrumbs={[{ label: 'Home', href: '/' }, { label: 'Inventory' }]}
      />
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-6">
        {kpis.map(kpi => (
          <div key={kpi.label} className="bg-white rounded-xl border border-gray-200 p-5">
            <p className="text-sm text-gray-500">{kpi.label}</p>
            <p className="text-2xl font-bold text-gray-900 mt-1">{kpi.value}</p>
            {kpi.change && <p className={`text-xs mt-1 ${kpi.change.startsWith('+') ? 'text-green-600' : 'text-red-600'}`}>{kpi.change} this week</p>}
          </div>
        ))}
      </div>
    </>
  );
}
