'use client';

import { useState, useEffect, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { PageHeader } from '@/components/layout/PageHeader';
import { DataTable, type Column } from '@/components/data/DataTable';
import { StatusBadge } from '@/components/data/StatusBadge';
import { Button } from '@/components/ui/Button';
import { formatCurrency, formatDate } from '@/lib/formatters';
import { inventoryApi } from '@/services/inventory.api';

const GRN_TABS = [
  { key: 'AgainstPO', label: 'With PO', icon: 'shopping_cart' },
  { key: 'WithoutPO', label: 'Without PO', icon: 'receipt' },
  { key: 'CustomerSupplied', label: 'Customer Supplied', icon: 'person' },
  { key: 'ReturnInward', label: 'Returnable DC', icon: 'assignment_return' },
] as const;

interface GrnRow {
  id: string;
  grnNumber: string;
  grnDate: string;
  grnType: string;
  status: string;
  vendorName?: string;
  customerName?: string;
  warehouseName?: string;
  totalGrnValue: number;
  lineItemCount: number;
}

const columns: Column<GrnRow>[] = [
  { key: 'grnNumber', header: 'GRN Number', width: '150px', render: (row) => <span className="font-medium text-blue-600">{row.grnNumber}</span> },
  { key: 'grnDate', header: 'Date', width: '110px', render: (row) => formatDate(row.grnDate) },
  { key: 'party', header: 'Vendor / Customer', render: (row) => row.vendorName || row.customerName || '-' },
  { key: 'warehouseName', header: 'Warehouse', width: '200px' },
  { key: 'lineItemCount', header: 'Items', width: '70px', render: (row) => <span className="text-center block">{row.lineItemCount}</span> },
  { key: 'totalGrnValue', header: 'Value', width: '130px', render: (row) => row.totalGrnValue > 0 ? formatCurrency(row.totalGrnValue) : <span className="text-gray-400">N/A</span> },
  { key: 'status', header: 'Status', width: '130px', render: (row) => <StatusBadge status={row.status} /> },
];

export default function GrnListPage() {
  const router = useRouter();
  const [activeTab, setActiveTab] = useState<string>('AgainstPO');
  const [showCreateMenu, setShowCreateMenu] = useState(false);
  const [data, setData] = useState<GrnRow[]>([]);
  const [allData, setAllData] = useState<GrnRow[]>([]);
  const [isLoading, setIsLoading] = useState(true);

  const fetchGrns = useCallback(async () => {
    setIsLoading(true);
    try {
      const res = await inventoryApi.grn.list({ pageSize: 100 });
      const items = (res.data as any)?.data?.items || [];
      setAllData(items.map((g: any) => ({
        id: g.id,
        grnNumber: g.grnNumber,
        grnDate: g.grnDate,
        grnType: g.grnType,
        status: g.status,
        vendorName: g.vendorName,
        customerName: g.customerName,
        warehouseName: g.warehouseName,
        totalGrnValue: g.totalGrnValue ?? 0,
        lineItemCount: g.lineItemCount ?? g.lineItems?.length ?? 0,
      })));
    } catch {
      // API not available — keep empty
      setAllData([]);
    }
    setIsLoading(false);
  }, []);

  useEffect(() => { fetchGrns(); }, [fetchGrns]);

  // Filter by active tab
  const filteredData = allData.filter(g => g.grnType === activeTab);
  const tabCounts = GRN_TABS.map(tab => ({
    ...tab,
    count: allData.filter(g => g.grnType === tab.key).length,
  }));

  const createOptions = [
    { key: 'AgainstPO', label: 'GRN - With PO', href: '/inventory/grn/create?type=with-po', icon: 'shopping_cart' },
    { key: 'WithoutPO', label: 'GRN - Without PO', href: '/inventory/grn/create?type=without-po', icon: 'receipt' },
    { key: 'CustomerSupplied', label: 'GRN - Customer Supplied', href: '/inventory/grn/create?type=customer-supplied', icon: 'person' },
    { key: 'ReturnInward', label: 'GRN - Returnable DC', href: '/inventory/grn/create?type=returnable-dc', icon: 'assignment_return' },
  ];

  // tabCounts already computed above

  return (
    <>
      <PageHeader
        title="Goods Receipt Notes"
        subtitle="Manage all inward goods receipts across all types"
        breadcrumbs={[{ label: 'Inventory', href: '/inventory' }, { label: 'Goods Receipt' }]}
        actions={
          <div className="relative">
            <Button onClick={() => setShowCreateMenu(!showCreateMenu)}>
              + Create GRN
            </Button>
            {showCreateMenu && (
              <div className="absolute right-0 mt-2 w-64 bg-white rounded-xl shadow-xl border border-gray-200 py-2 z-50">
                {createOptions.map(opt => (
                  <button
                    key={opt.key}
                    className="w-full flex items-center gap-3 px-4 py-3 text-sm text-gray-700 hover:bg-gray-50 transition-colors"
                    onClick={() => { router.push(opt.href); setShowCreateMenu(false); }}
                  >
                    <span className="material-symbols-outlined text-lg text-gray-400">{opt.icon}</span>
                    {opt.label}
                  </button>
                ))}
              </div>
            )}
          </div>
        }
      />

      {/* Type Tabs */}
      <div className="flex items-center gap-1 mb-6 bg-white rounded-xl border border-gray-200 p-1.5">
        {tabCounts.map(tab => (
          <button
            key={tab.key}
            onClick={() => setActiveTab(tab.key)}
            className={`flex items-center gap-2 px-4 py-2.5 rounded-lg text-sm font-medium transition-all ${
              activeTab === tab.key
                ? 'bg-blue-50 text-blue-700 shadow-sm'
                : 'text-gray-500 hover:text-gray-700 hover:bg-gray-50'
            }`}
          >
            <span
              className="material-symbols-outlined text-lg"
              style={activeTab === tab.key ? { fontVariationSettings: "'FILL' 1" } : undefined}
            >
              {tab.icon}
            </span>
            {tab.label}
            <span className={`px-2 py-0.5 rounded-full text-xs font-bold ${
              activeTab === tab.key ? 'bg-blue-100 text-blue-700' : 'bg-gray-100 text-gray-500'
            }`}>
              {tab.count}
            </span>
          </button>
        ))}
      </div>

      {/* Data Table */}
      <DataTable
        columns={columns}
        data={filteredData}
        isLoading={isLoading}
        onRowClick={(row) => router.push(`/inventory/grn/${row.id}`)}
        keyExtractor={(row) => row.id}
        emptyMessage={`No ${GRN_TABS.find(t => t.key === activeTab)?.label} GRNs found`}
      />
    </>
  );
}
