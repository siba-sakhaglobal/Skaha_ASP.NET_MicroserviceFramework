'use client';

import { useParams, useRouter } from 'next/navigation';
import { PageHeader } from '@/components/layout/PageHeader';
import { Button } from '@/components/ui/Button';
import { StatusBadge } from '@/components/data/StatusBadge';
import { formatCurrency, formatDate, formatDateTime, formatNumber } from '@/lib/formatters';

// ── Types ────────────────────────────────────────────────────────────────────

interface GrnLineItemDetail {
  id: string;
  itemCode: string;
  itemName: string;
  uom: string;
  orderedQuantity: number;
  receivedQuantity: number;
  acceptedQuantity: number;
  rejectedQuantity: number;
  unitRate: number;
  taxRate: number;
  basicAmount: number;
  taxAmount: number;
  additionalCharges: number;
  totalAmount: number;
  warehouseName: string;
  binCode?: string;
  batchNumber?: string;
  tagNumber?: string;
  qcRequired: boolean;
  qcStatus: string;
  conditionOnReturn?: string;
  remarks?: string;
}

interface GrnDetail {
  id: string;
  grnNumber: string;
  grnDate: string;
  grnType: string;
  status: string;
  siteName: string;
  warehouseName: string;
  vendorName?: string;
  customerName?: string;
  contactPerson?: string;
  contactEmail?: string;
  contactPhone?: string;
  supplierChallanNo?: string;
  supplierChallanDate?: string;
  gateInwardNo?: string;
  internalRefNo?: string;
  receiptReason?: string;
  exchangeRate: number;
  basicValue: number;
  taxAmount: number;
  additionalChargesTotal: number;
  totalGrnValue: number;
  remarks?: string;
  // Logistics
  transporterName?: string;
  vehicleNumber?: string;
  driverName?: string;
  driverContact?: string;
  ewayBillNumber?: string;
  // Attachments (JSON array string)
  attachments?: { name: string; size: string; uploadedAt: string }[];
  // Audit
  createdByName: string;
  createdAt: string;
  lastModifiedAt: string;
  lineItems: GrnLineItemDetail[];
}

// ── Mock Data ────────────────────────────────────────────────────────────────

const MOCK_GRN: GrnDetail = {
  id: '1',
  grnNumber: 'GRN-2026-03-0001',
  grnDate: '2026-03-28',
  grnType: 'AgainstPO',
  status: 'Completed',
  siteName: 'Mumbai Main Plant',
  warehouseName: 'Warehouse A - Raw Materials',
  vendorName: 'Tata Steel Ltd.',
  contactPerson: 'Rajesh Sharma',
  contactEmail: 'rajesh.sharma@tatasteel.com',
  contactPhone: '+91 98765 43210',
  supplierChallanNo: 'TS-DC-2026-04512',
  supplierChallanDate: '2026-03-27',
  gateInwardNo: 'GI-2026-03-0089',
  internalRefNo: 'PO-2026-02-0034',
  receiptReason: 'Standard Purchase Order Receipt',
  exchangeRate: 1,
  basicValue: 780000,
  taxAmount: 140400,
  additionalChargesTotal: 12500,
  totalGrnValue: 932900,
  remarks: 'All materials inspected and accepted. Minor surface rust on 2 coils — accepted with vendor concession note CN-2026-089.',
  // Logistics
  transporterName: 'Blue Dart Logistics Pvt. Ltd.',
  vehicleNumber: 'MH 04 BT 9921',
  driverName: 'Suresh Kumar',
  driverContact: '+91 97654 32109',
  ewayBillNumber: 'EWB-2026-03-112233',
  // Attachments
  attachments: [
    { name: 'Supplier_Challan_TS-DC-2026-04512.pdf', size: '245 KB', uploadedAt: '2026-03-28T10:15:00' },
    { name: 'Inspection_Report_GRN-0001.pdf', size: '512 KB', uploadedAt: '2026-03-28T11:30:00' },
    { name: 'EWayBill_EWB-2026-03-112233.pdf', size: '89 KB', uploadedAt: '2026-03-28T09:45:00' },
  ],
  // Audit
  createdByName: 'Priya Mehta',
  createdAt: '2026-03-28T09:30:00',
  lastModifiedAt: '2026-03-28T14:45:00',
  lineItems: [
    {
      id: 'li-1',
      itemCode: 'RM-STEEL-HR-3MM',
      itemName: 'Hot Rolled Steel Coil 3mm',
      uom: 'MT',
      orderedQuantity: 50,
      receivedQuantity: 50,
      acceptedQuantity: 50,
      rejectedQuantity: 0,
      unitRate: 8200,
      taxRate: 18,
      basicAmount: 410000,
      taxAmount: 73800,
      additionalCharges: 5000,
      totalAmount: 488800,
      warehouseName: 'Warehouse A - Raw Materials',
      binCode: 'BIN-A-01-03',
      batchNumber: 'BATCH-TS-2026-0312',
      tagNumber: 'TAG-001',
      qcRequired: true,
      qcStatus: 'Passed',
      remarks: 'Coils in good condition.',
    },
    {
      id: 'li-2',
      itemCode: 'RM-STEEL-HR-5MM',
      itemName: 'Hot Rolled Steel Coil 5mm',
      uom: 'MT',
      orderedQuantity: 40,
      receivedQuantity: 40,
      acceptedQuantity: 40,
      rejectedQuantity: 0,
      unitRate: 9200,
      taxRate: 18,
      basicAmount: 368000,
      taxAmount: 66240,
      additionalCharges: 7500,
      totalAmount: 441740,
      warehouseName: 'Warehouse A - Raw Materials',
      binCode: 'BIN-A-01-04',
      batchNumber: 'BATCH-TS-2026-0313',
      tagNumber: 'TAG-002',
      qcRequired: true,
      qcStatus: 'Passed',
    },
    {
      id: 'li-3',
      itemCode: 'RM-STEEL-HR-8MM',
      itemName: 'Hot Rolled Steel Plate 8mm',
      uom: 'MT',
      orderedQuantity: 2.5,
      receivedQuantity: 2.5,
      acceptedQuantity: 2.5,
      rejectedQuantity: 0,
      unitRate: 8000,
      taxRate: 18,
      basicAmount: 20000,
      taxAmount: 3600,
      additionalCharges: 0,
      totalAmount: 23600,
      warehouseName: 'Warehouse A - Raw Materials',
      binCode: 'BIN-A-02-01',
      qcRequired: false,
      qcStatus: 'NotRequired',
    },
  ],
};

// ── Helper Components ────────────────────────────────────────────────────────

function SectionCard({ title, icon, children }: { title: string; icon: string; children: React.ReactNode }) {
  return (
    <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <div className="flex items-center gap-2.5 px-5 py-3.5 border-b border-gray-100 bg-gray-50">
        <span className="material-symbols-outlined text-[18px] text-gray-500">{icon}</span>
        <h2 className="text-sm font-semibold text-gray-700 uppercase tracking-wide">{title}</h2>
      </div>
      <div className="p-5">{children}</div>
    </div>
  );
}

function DetailGrid({ children }: { children: React.ReactNode }) {
  return <dl className="grid grid-cols-2 sm:grid-cols-3 lg:grid-cols-4 gap-x-6 gap-y-4">{children}</dl>;
}

function DetailField({ label, value, span }: { label: string; value?: React.ReactNode; span?: number }) {
  return (
    <div className={span === 2 ? 'col-span-2' : undefined}>
      <dt className="text-xs font-medium text-gray-400 uppercase tracking-wider mb-0.5">{label}</dt>
      <dd className="text-sm text-gray-800 font-medium">{value ?? <span className="text-gray-300">—</span>}</dd>
    </div>
  );
}

function GrnTypeBadge({ type }: { type: string }) {
  const map: Record<string, { label: string; cls: string }> = {
    AgainstPO: { label: 'With PO', cls: 'bg-blue-50 text-blue-700 border border-blue-200' },
    WithoutPO: { label: 'Without PO', cls: 'bg-purple-50 text-purple-700 border border-purple-200' },
    CustomerSupplied: { label: 'Customer Supplied', cls: 'bg-teal-50 text-teal-700 border border-teal-200' },
    ReturnInward: { label: 'Returnable DC', cls: 'bg-orange-50 text-orange-700 border border-orange-200' },
  };
  const { label, cls } = map[type] ?? { label: type, cls: 'bg-gray-100 text-gray-600' };
  return <span className={`inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium ${cls}`}>{label}</span>;
}

function QcBadge({ status }: { status: string }) {
  const map: Record<string, string> = {
    NotRequired: 'bg-gray-100 text-gray-500',
    Pending: 'bg-yellow-50 text-yellow-700',
    Passed: 'bg-green-50 text-green-700',
    Failed: 'bg-red-50 text-red-700',
  };
  return (
    <span className={`inline-flex rounded-full px-2 py-0.5 text-xs font-medium ${map[status] ?? 'bg-gray-100 text-gray-500'}`}>
      {status}
    </span>
  );
}

// ── Page ─────────────────────────────────────────────────────────────────────

export default function GrnDetailPage() {
  const params = useParams();
  const router = useRouter();
  const id = params?.id as string;

  // In production: fetch from API using `id`. Using mock data for now.
  const grn: GrnDetail = MOCK_GRN;

  const partyLabel = ['AgainstPO', 'WithoutPO'].includes(grn.grnType) ? 'Vendor' : 'Customer';
  const partyName = grn.vendorName ?? grn.customerName;

  return (
    <>
      <PageHeader
        title={grn.grnNumber}
        breadcrumbs={[
          { label: 'Inventory', href: '/inventory' },
          { label: 'Goods Receipt Notes', href: '/inventory/grn' },
          { label: grn.grnNumber },
        ]}
        actions={
          <div className="flex items-center gap-2">
            <GrnTypeBadge type={grn.grnType} />
            <StatusBadge status={grn.status} />
            {grn.status === 'Draft' && (
              <Button variant="outline" size="sm" onClick={() => router.push(`/inventory/grn/${id}/edit`)}>
                <span className="material-symbols-outlined text-[16px] mr-1.5">edit</span>
                Edit Draft
              </Button>
            )}
            {grn.status === 'Draft' && (
              <Button size="sm">
                <span className="material-symbols-outlined text-[16px] mr-1.5">publish</span>
                Post GRN
              </Button>
            )}
            <Button variant="outline" size="sm">
              <span className="material-symbols-outlined text-[16px] mr-1.5">print</span>
              Print
            </Button>
            {grn.status === 'Draft' && (
              <Button variant="danger" size="sm">
                <span className="material-symbols-outlined text-[16px] mr-1.5">cancel</span>
                Cancel
              </Button>
            )}
          </div>
        }
      />

      <div className="space-y-5">

        {/* ── GRN Details ── */}
        <SectionCard title="GRN Details" icon="receipt_long">
          <DetailGrid>
            <DetailField label="GRN Date" value={formatDate(grn.grnDate, 'long')} />
            <DetailField label="GRN Type" value={<GrnTypeBadge type={grn.grnType} />} />
            <DetailField label="Status" value={<StatusBadge status={grn.status} />} />
            <DetailField label="Site" value={grn.siteName} />
            <DetailField label="Warehouse" value={grn.warehouseName} />
            <DetailField label={partyLabel} value={partyName} />
            {grn.contactPerson && <DetailField label="Contact Person" value={grn.contactPerson} />}
            {grn.contactEmail && <DetailField label="Contact Email" value={grn.contactEmail} />}
            {grn.contactPhone && <DetailField label="Contact Phone" value={grn.contactPhone} />}
            <DetailField label="Supplier Challan No." value={grn.supplierChallanNo} />
            <DetailField label="Challan Date" value={grn.supplierChallanDate ? formatDate(grn.supplierChallanDate, 'long') : undefined} />
            <DetailField label="Gate Inward Ref." value={grn.gateInwardNo} />
            <DetailField label="Internal Ref. No." value={grn.internalRefNo} />
            {grn.exchangeRate !== 1 && <DetailField label="Exchange Rate" value={formatNumber(grn.exchangeRate, 4)} />}
            {grn.receiptReason && <DetailField label="Receipt Reason" value={grn.receiptReason} span={2} />}
          </DetailGrid>
        </SectionCard>

        {/* ── Line Items ── */}
        <SectionCard title={`Line Items (${grn.lineItems.length})`} icon="inventory_2">
          <div className="overflow-x-auto -mx-5">
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-gray-50 border-b border-gray-200">
                  {['#', 'Item Code', 'Item Description', 'UOM', 'Ordered', 'Received', 'Accepted', 'Rejected', 'Unit Rate', 'Tax %', 'Basic Amt', 'Tax Amt', 'Total Amt', 'Bin', 'Batch', 'QC Status'].map(h => (
                    <th key={h} className="px-4 py-2.5 text-left text-xs font-semibold text-gray-500 uppercase tracking-wide whitespace-nowrap">{h}</th>
                  ))}
                </tr>
              </thead>
              <tbody className="divide-y divide-gray-100">
                {grn.lineItems.map((item, idx) => (
                  <tr key={item.id} className="hover:bg-gray-50 transition-colors">
                    <td className="px-4 py-3 text-gray-500 text-center">{idx + 1}</td>
                    <td className="px-4 py-3 font-mono text-xs text-blue-700 whitespace-nowrap">{item.itemCode}</td>
                    <td className="px-4 py-3 text-gray-800 min-w-[200px]">
                      <div className="font-medium">{item.itemName}</div>
                      {item.tagNumber && <div className="text-xs text-gray-400 mt-0.5">Tag: {item.tagNumber}</div>}
                      {item.remarks && <div className="text-xs text-gray-400 mt-0.5 italic">{item.remarks}</div>}
                    </td>
                    <td className="px-4 py-3 text-gray-600 whitespace-nowrap">{item.uom}</td>
                    <td className="px-4 py-3 text-right tabular-nums">{formatNumber(item.orderedQuantity, 3)}</td>
                    <td className="px-4 py-3 text-right tabular-nums">{formatNumber(item.receivedQuantity, 3)}</td>
                    <td className="px-4 py-3 text-right tabular-nums text-green-700 font-medium">{formatNumber(item.acceptedQuantity, 3)}</td>
                    <td className="px-4 py-3 text-right tabular-nums text-red-600">{formatNumber(item.rejectedQuantity, 3)}</td>
                    <td className="px-4 py-3 text-right tabular-nums">{formatCurrency(item.unitRate)}</td>
                    <td className="px-4 py-3 text-right tabular-nums">{item.taxRate}%</td>
                    <td className="px-4 py-3 text-right tabular-nums">{formatCurrency(item.basicAmount)}</td>
                    <td className="px-4 py-3 text-right tabular-nums">{formatCurrency(item.taxAmount)}</td>
                    <td className="px-4 py-3 text-right tabular-nums font-semibold">{formatCurrency(item.totalAmount)}</td>
                    <td className="px-4 py-3 text-gray-600 whitespace-nowrap text-xs">{item.binCode ?? '—'}</td>
                    <td className="px-4 py-3 text-gray-600 whitespace-nowrap text-xs">{item.batchNumber ?? '—'}</td>
                    <td className="px-4 py-3 whitespace-nowrap"><QcBadge status={item.qcStatus} /></td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </SectionCard>

        {/* ── Logistics ── */}
        {(grn.transporterName || grn.vehicleNumber || grn.driverName || grn.ewayBillNumber) && (
          <SectionCard title="Logistics Details" icon="local_shipping">
            <DetailGrid>
              <DetailField label="Transporter" value={grn.transporterName} />
              <DetailField label="Vehicle Number" value={grn.vehicleNumber} />
              <DetailField label="Driver Name" value={grn.driverName} />
              <DetailField label="Driver Contact" value={grn.driverContact} />
              <DetailField label="E-Way Bill No." value={grn.ewayBillNumber} />
            </DetailGrid>
          </SectionCard>
        )}

        {/* ── Financial Summary ── */}
        <SectionCard title="Financial Summary" icon="calculate">
          <div className="flex justify-end">
            <dl className="w-full max-w-sm space-y-2">
              <div className="flex justify-between text-sm">
                <dt className="text-gray-500">Basic Value</dt>
                <dd className="font-medium tabular-nums">{formatCurrency(grn.basicValue)}</dd>
              </div>
              <div className="flex justify-between text-sm">
                <dt className="text-gray-500">Tax Amount (GST)</dt>
                <dd className="font-medium tabular-nums">{formatCurrency(grn.taxAmount)}</dd>
              </div>
              <div className="flex justify-between text-sm">
                <dt className="text-gray-500">Additional Charges</dt>
                <dd className="font-medium tabular-nums">{formatCurrency(grn.additionalChargesTotal)}</dd>
              </div>
              {grn.exchangeRate !== 1 && (
                <div className="flex justify-between text-sm">
                  <dt className="text-gray-500">Exchange Rate</dt>
                  <dd className="font-medium tabular-nums">{formatNumber(grn.exchangeRate, 4)}</dd>
                </div>
              )}
              <div className="border-t border-gray-200 pt-2 flex justify-between text-base">
                <dt className="font-semibold text-gray-800">Total GRN Value</dt>
                <dd className="font-bold text-gray-900 tabular-nums">{formatCurrency(grn.totalGrnValue)}</dd>
              </div>
            </dl>
          </div>
        </SectionCard>

        {/* ── Attachments ── */}
        {grn.attachments && grn.attachments.length > 0 && (
          <SectionCard title={`Attachments (${grn.attachments.length})`} icon="attach_file">
            <ul className="space-y-2">
              {grn.attachments.map((att, i) => (
                <li key={i} className="flex items-center gap-3 p-3 rounded-lg border border-gray-100 bg-gray-50 hover:bg-gray-100 transition-colors">
                  <span className="material-symbols-outlined text-[20px] text-red-500">picture_as_pdf</span>
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-gray-800 truncate">{att.name}</p>
                    <p className="text-xs text-gray-400">{att.size} · Uploaded {formatDateTime(att.uploadedAt)}</p>
                  </div>
                  <button className="text-blue-600 hover:text-blue-800 text-sm font-medium transition-colors">
                    Download
                  </button>
                </li>
              ))}
            </ul>
          </SectionCard>
        )}

        {/* ── Remarks ── */}
        {grn.remarks && (
          <SectionCard title="Remarks" icon="notes">
            <p className="text-sm text-gray-700 leading-relaxed">{grn.remarks}</p>
          </SectionCard>
        )}

        {/* ── Audit Trail ── */}
        <SectionCard title="Audit Trail" icon="history">
          <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
            <div className="flex items-start gap-3">
              <span className="material-symbols-outlined text-[20px] text-gray-400 mt-0.5">person</span>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide mb-0.5">Created By</p>
                <p className="text-sm font-medium text-gray-800">{grn.createdByName}</p>
              </div>
            </div>
            <div className="flex items-start gap-3">
              <span className="material-symbols-outlined text-[20px] text-gray-400 mt-0.5">schedule</span>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide mb-0.5">Created At</p>
                <p className="text-sm font-medium text-gray-800">{formatDateTime(grn.createdAt)}</p>
              </div>
            </div>
            <div className="flex items-start gap-3">
              <span className="material-symbols-outlined text-[20px] text-gray-400 mt-0.5">edit_calendar</span>
              <div>
                <p className="text-xs text-gray-400 uppercase tracking-wide mb-0.5">Last Modified</p>
                <p className="text-sm font-medium text-gray-800">{formatDateTime(grn.lastModifiedAt)}</p>
              </div>
            </div>
          </div>
        </SectionCard>

      </div>
    </>
  );
}
