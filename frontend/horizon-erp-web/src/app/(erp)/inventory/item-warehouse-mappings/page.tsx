'use client';

import { useState, useEffect } from 'react';
import { useRouter } from 'next/navigation';
import { PageHeader } from '@/components/layout/PageHeader';
import { DataTable, type Column } from '@/components/data/DataTable';
import { StatusBadge } from '@/components/data/StatusBadge';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { inventoryApi } from '@/services/inventory.api';
import { formatNumber } from '@/lib/formatters';
import type { ItemWarehouseMapping } from '@/types/inventory.types';

const columns: Column<ItemWarehouseMapping>[] = [
  { key: 'mappingCode', header: 'Code', width: '140px' },
  { key: 'itemCode', header: 'Item Code', width: '120px' },
  { key: 'itemName', header: 'Item Name' },
  { key: 'warehouseCode', header: 'Warehouse', width: '120px' },
  { key: 'binName', header: 'Bin', width: '100px', render: (row) => row.binName || '-' },
  { key: 'status', header: 'Status', width: '100px', render: (row) => <StatusBadge status={row.status} /> },
  { key: 'minQuantity', header: 'Min Qty', width: '90px', render: (row) => row.minQuantity != null ? formatNumber(row.minQuantity, 0) : '-' },
  { key: 'maxQuantity', header: 'Max Qty', width: '90px', render: (row) => row.maxQuantity != null ? formatNumber(row.maxQuantity, 0) : '-' },
];

export default function ItemWarehouseMappingsPage() {
  const router = useRouter();
  const [data, setData] = useState<ItemWarehouseMapping[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [search, setSearch] = useState('');

  useEffect(() => {
    loadData();
  }, []);

  const loadData = async () => {
    setIsLoading(true);
    try {
      const res = await inventoryApi.itemWarehouseMappings.list({ search });
      setData(res.data.data?.items || []);
    } catch { setData([]); }
    setIsLoading(false);
  };

  return (
    <>
      <PageHeader
        title="Item-Warehouse Mappings"
        subtitle="Manage which items are authorized in which warehouses"
        breadcrumbs={[{ label: 'Inventory', href: '/inventory' }, { label: 'Item-Warehouse Mappings' }]}
        actions={<Button onClick={() => router.push('/inventory/item-warehouse-mappings/new')}>+ New Mapping</Button>}
      />
      <div className="mb-4">
        <Input placeholder="Search by item code, name, or warehouse..." value={search} onChange={e => setSearch(e.target.value)} className="max-w-md" />
      </div>
      <DataTable columns={columns} data={data} isLoading={isLoading} onRowClick={(row) => router.push(`/inventory/item-warehouse-mappings/${row.id}`)} keyExtractor={(row) => row.id} />
    </>
  );
}
