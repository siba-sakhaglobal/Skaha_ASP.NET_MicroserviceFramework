'use client';

import { useState, useCallback } from 'react';
import { useRouter } from 'next/navigation';
import { PageHeader } from '@/components/layout/PageHeader';
import { Button } from '@/components/ui/Button';
import { formatCurrency } from '@/lib/formatters';

/* ------------------------------------------------------------------ */
/*  Types                                                              */
/* ------------------------------------------------------------------ */
interface LineItem {
  id: string;
  poNumber: string;
  itemName: string;
  itemDescription: string;
  itemCode: string;
  uom: string;
  orderedQty: number;
  receivedQty: number;
  pendingQty: number;
  qcStatus: 'Pending' | 'Passed' | 'Failed';
  unitRate: number;
  taxPercent: number;
  taxAmount: number;
  lineTotal: number;
  remarks: string;
}

/* ------------------------------------------------------------------ */
/*  Mock data                                                          */
/* ------------------------------------------------------------------ */
const INITIAL_LINES: LineItem[] = [
  {
    id: '1', poNumber: 'PO-2026-0112', itemName: 'MS Flat Bar 50x6mm', itemDescription: 'Mild steel flat bar, IS 2062 grade, 50x6mm, 6m length',
    itemCode: 'RM-STL-001', uom: 'Tons', orderedQty: 10, receivedQty: 8, pendingQty: 2, qcStatus: 'Pending',
    unitRate: 52000, taxPercent: 18, taxAmount: 74880, lineTotal: 490880, remarks: '',
  },
  {
    id: '2', poNumber: 'PO-2026-0112', itemName: 'Hex Bolt M16x60 SS304', itemDescription: 'Stainless steel hex bolt, M16x60mm, Grade A2-70',
    itemCode: 'HW-FAS-042', uom: 'Boxes', orderedQty: 50, receivedQty: 50, pendingQty: 0, qcStatus: 'Passed',
    unitRate: 1200, taxPercent: 18, taxAmount: 10800, lineTotal: 70800, remarks: '',
  },
  {
    id: '3', poNumber: 'PO-2026-0115', itemName: 'Hydraulic Oil ISO VG 68', itemDescription: 'Premium hydraulic oil, ISO VG 68, 20L drum',
    itemCode: 'MN-OIL-015', uom: 'Drums', orderedQty: 20, receivedQty: 15, pendingQty: 5, qcStatus: 'Pending',
    unitRate: 4500, taxPercent: 18, taxAmount: 12150, lineTotal: 79650, remarks: '',
  },
];

const PO_OPTIONS = [
  { value: 'PO-2026-0112', label: 'PO-2026-0112 - Tata Steel Ltd. (28 Mar 2026)' },
  { value: 'PO-2026-0115', label: 'PO-2026-0115 - Tata Steel Ltd. (25 Mar 2026)' },
  { value: 'PO-2026-0118', label: 'PO-2026-0118 - Hindalco Industries (30 Mar 2026)' },
];

const VENDOR_LIST = [
  { value: 'V001', label: 'Tata Steel Ltd.' },
  { value: 'V002', label: 'Hindalco Industries' },
  { value: 'V003', label: 'JSW Steel' },
];

/* ------------------------------------------------------------------ */
/*  Helpers                                                            */
/* ------------------------------------------------------------------ */
function Section({ icon, title, children, color = 'blue' }: { icon: string; title: string; children: React.ReactNode; color?: 'blue' | 'emerald' | 'violet' | 'amber' | 'rose' | 'indigo' }) {
  const colorMap: Record<string, string> = {
    blue: 'bg-blue-50 text-blue-600',
    emerald: 'bg-emerald-50 text-emerald-600',
    violet: 'bg-violet-50 text-violet-600',
    amber: 'bg-amber-50 text-amber-600',
    rose: 'bg-rose-50 text-rose-600',
    indigo: 'bg-indigo-50 text-indigo-600',
  };
  return (
    <div className="bg-white rounded-2xl border border-gray-200 overflow-hidden">
      <div className={`flex items-center gap-2.5 px-5 py-3.5 border-b border-gray-100 ${colorMap[color]}`}>
        <span className="material-symbols-outlined text-[20px]">{icon}</span>
        <h3 className="text-[11px] font-bold uppercase tracking-widest">{title}</h3>
      </div>
      <div className="p-5">{children}</div>
    </div>
  );
}

function FieldLabel({ children, required }: { children: React.ReactNode; required?: boolean }) {
  return (
    <label className="block text-[11px] font-bold uppercase tracking-tighter text-gray-500 mb-1.5">
      {children}{required && <span className="text-red-500 ml-0.5">*</span>}
    </label>
  );
}

function QcBadge({ status }: { status: 'Pending' | 'Passed' | 'Failed' }) {
  const map = { Pending: 'bg-yellow-100 text-yellow-700', Passed: 'bg-green-100 text-green-700', Failed: 'bg-red-100 text-red-700' };
  const dotMap = { Pending: 'bg-yellow-500', Passed: 'bg-green-500', Failed: 'bg-red-500' };
  return (
    <span className={`inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium ${map[status]}`}>
      <span className={`h-1.5 w-1.5 rounded-full ${dotMap[status]}`} />
      {status}
    </span>
  );
}

/* ================================================================== */
/*  MAIN COMPONENT                                                     */
/* ================================================================== */
export default function GrnWithPOForm() {
  const router = useRouter();

  // Header
  const [grnDate, setGrnDate] = useState('2026-03-31');
  const [warehouse, setWarehouse] = useState('WH-A');
  const [createdBy] = useState('Arjun Krishnamurthy');

  // Vendor
  const [vendorSearch, setVendorSearch] = useState('Tata Steel Ltd.');
  const [vendorName] = useState('Tata Steel Limited');
  const [vendorGstin] = useState('27AAACT2727F1ZN');
  const [vendorAddress] = useState('Bombay House, 24 Homi Mody Street, Fort, Mumbai - 400001, Maharashtra');

  // PO Selection
  const [selectedPOs, setSelectedPOs] = useState<string[]>(['PO-2026-0112', 'PO-2026-0115']);

  // Line Items
  const [lineItems, setLineItems] = useState<LineItem[]>(INITIAL_LINES);

  // Logistics
  const [challanNo, setChallanNo] = useState('TS/INV/2026/4521');
  const [challanDate, setChallanDate] = useState('2026-03-29');
  const [gateEntry, setGateEntry] = useState('GE-2026-03-0145');
  const [gateEntryDate] = useState('2026-03-31');
  const [transporterName, setTransporterName] = useState('GATI Logistics');
  const [vehicleNumber, setVehicleNumber] = useState('MH 04 CD 5678');
  const [driverInfo, setDriverInfo] = useState('Suresh Patil / +91 99876 54321');
  const [ewayBill, setEwayBill] = useState('3214567890123');

  // Attachments & Remarks
  const [files, setFiles] = useState<{ name: string; size: string }[]>([
    { name: 'invoice_TS_4521.pdf', size: '1.8 MB' },
  ]);
  const [remarks, setRemarks] = useState('');
  const [isDragging, setIsDragging] = useState(false);

  const updateLine = useCallback((id: string, field: keyof LineItem, value: any) => {
    setLineItems(prev =>
      prev.map(l => {
        if (l.id !== id) return l;
        const updated = { ...l, [field]: value };
        if (field === 'receivedQty') {
          updated.pendingQty = updated.orderedQty - Number(value);
          const base = Number(value) * updated.unitRate;
          updated.taxAmount = base * (updated.taxPercent / 100);
          updated.lineTotal = base + updated.taxAmount;
        }
        return updated;
      }),
    );
  }, []);

  const removeLine = useCallback((id: string) => {
    setLineItems(prev => prev.filter(l => l.id !== id));
  }, []);

  const removeFile = useCallback((idx: number) => {
    setFiles(prev => prev.filter((_, i) => i !== idx));
  }, []);

  // Computed
  const totalItems = lineItems.length;
  const totalQty = lineItems.reduce((s, l) => s + l.receivedQty, 0);
  const basicValue = lineItems.reduce((s, l) => s + l.receivedQty * l.unitRate, 0);
  const totalTax = lineItems.reduce((s, l) => s + l.taxAmount, 0);
  const totalValue = lineItems.reduce((s, l) => s + l.lineTotal, 0);

  return (
    <>
      <PageHeader
        title="Create GRN - With Purchase Order"
        subtitle="Receive goods against approved purchase orders"
        breadcrumbs={[
          { label: 'Inventory', href: '/inventory' },
          { label: 'Goods Receipt', href: '/inventory/grn' },
          { label: 'Create - With PO' },
        ]}
      />

      <div className="space-y-6 pb-32">
        {/* 1. HEADER INFORMATION */}
        <Section icon="description" title="Header Information" color="blue">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
            <div>
              <FieldLabel>GRN Number</FieldLabel>
              <input type="text" value="GRN-2026-03-AUTO" readOnly className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed" />
              <p className="text-[10px] text-gray-400 mt-1">Auto-generated on save</p>
            </div>
            <div>
              <FieldLabel required>GRN Date</FieldLabel>
              <input type="date" value={grnDate} onChange={(e) => setGrnDate(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>GRN Type</FieldLabel>
              <div className="flex items-center gap-2 rounded-md border border-blue-200 bg-blue-50 px-3 py-2">
                <span className="material-symbols-outlined text-blue-600 text-lg">shopping_cart</span>
                <span className="text-sm font-medium text-blue-700">With Purchase Order</span>
              </div>
            </div>
            <div>
              <FieldLabel>Status</FieldLabel>
              <span className="inline-flex items-center gap-1.5 rounded-full bg-gray-100 px-3 py-2 text-xs font-medium text-gray-600">
                <span className="h-2 w-2 rounded-full bg-gray-400" />Draft
              </span>
            </div>
            <div>
              <FieldLabel>Site</FieldLabel>
              <input type="text" value="Main Manufacturing Unit - Pune" readOnly className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed" />
            </div>
            <div>
              <FieldLabel required>Receiving Warehouse</FieldLabel>
              <select value={warehouse} onChange={(e) => setWarehouse(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent">
                <option value="WH-A">Warehouse A - Raw Materials</option>
                <option value="WH-B">Warehouse B - Components</option>
                <option value="WH-C">Warehouse C - Consumables</option>
              </select>
            </div>
            <div>
              <FieldLabel>Created By</FieldLabel>
              <div className="flex items-center gap-2 rounded-md border border-gray-200 bg-gray-50 px-3 py-2">
                <div className="h-6 w-6 rounded-full bg-blue-100 flex items-center justify-center text-xs font-bold text-blue-600">AK</div>
                <span className="text-sm text-gray-700">{createdBy}</span>
              </div>
            </div>
          </div>
        </Section>

        {/* 2. VENDOR INFORMATION */}
        <Section icon="store" title="Vendor Information" color="emerald">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            <div className="lg:col-span-2">
              <FieldLabel required>Search Vendor</FieldLabel>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</span>
                <input type="text" value={vendorSearch} onChange={(e) => setVendorSearch(e.target.value)} placeholder="Type vendor name, code or GSTIN..." className="block w-full rounded-md border border-gray-300 pl-10 pr-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent" />
              </div>
            </div>
            <div>
              <FieldLabel>GSTIN</FieldLabel>
              <input type="text" value={vendorGstin} readOnly className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed font-mono" />
            </div>
            <div className="lg:col-span-2">
              <FieldLabel>Vendor Name</FieldLabel>
              <input type="text" value={vendorName} readOnly className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-700 cursor-not-allowed" />
            </div>
            <div className="lg:col-span-3">
              <FieldLabel>Vendor Address</FieldLabel>
              <textarea value={vendorAddress} readOnly rows={2} className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed resize-none" />
            </div>
          </div>
        </Section>

        {/* 3. PO SELECTION */}
        <Section icon="receipt_long" title="Purchase Order Selection" color="indigo">
          <div>
            <FieldLabel required>Select Purchase Orders</FieldLabel>
            <div className="flex flex-wrap gap-2 mb-3">
              {selectedPOs.map(po => (
                <span key={po} className="inline-flex items-center gap-1.5 rounded-full bg-indigo-100 text-indigo-700 px-3 py-1 text-xs font-medium">
                  {po}
                  <button type="button" onClick={() => setSelectedPOs(prev => prev.filter(p => p !== po))} className="hover:text-indigo-900">
                    <span className="material-symbols-outlined text-sm">close</span>
                  </button>
                </span>
              ))}
            </div>
            <select
              onChange={(e) => { if (e.target.value && !selectedPOs.includes(e.target.value)) setSelectedPOs(prev => [...prev, e.target.value]); e.target.value = ''; }}
              className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:border-transparent"
            >
              <option value="">-- Add Purchase Order --</option>
              {PO_OPTIONS.filter(o => !selectedPOs.includes(o.value)).map(o => (
                <option key={o.value} value={o.value}>{o.label}</option>
              ))}
            </select>
          </div>
        </Section>

        {/* 4. LINE ITEMS TABLE */}
        <Section icon="list_alt" title="Line Items - PO Material Receipt" color="violet">
          <div className="overflow-x-auto -mx-5">
            <table className="w-full text-sm min-w-[1200px]">
              <thead>
                <tr className="bg-gray-50 border-y border-gray-200">
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-10">#</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">PO Ref</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500">Item</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">UOM</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">Ordered</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Recv Qty</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">Pending</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">QC</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Rate</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">Tax %</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">Line Total</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-16">Actions</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map((line, idx) => (
                  <tr key={line.id} className={`border-b border-gray-100 ${idx % 2 === 1 ? 'bg-gray-50/50' : 'bg-white'} hover:bg-blue-50/30 transition-colors`}>
                    <td className="px-3 py-2.5 text-gray-400 font-medium">{idx + 1}</td>
                    <td className="px-3 py-2.5"><span className="text-xs font-mono text-indigo-600">{line.poNumber}</span></td>
                    <td className="px-3 py-2.5">
                      <span className="text-sm text-gray-800 font-medium">{line.itemName}</span>
                      <p className="text-[10px] text-gray-400 mt-0.5">{line.itemCode} - {line.itemDescription}</p>
                    </td>
                    <td className="px-3 py-2.5 text-xs text-gray-600">{line.uom}</td>
                    <td className="px-3 py-2.5 text-right text-xs text-gray-500">{line.orderedQty}</td>
                    <td className="px-3 py-2.5">
                      <input type="number" value={line.receivedQty || ''} onChange={(e) => updateLine(line.id, 'receivedQty', Number(e.target.value))} min={0} max={line.orderedQty} className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 bg-transparent" />
                    </td>
                    <td className="px-3 py-2.5 text-right">
                      <span className={`text-xs font-medium ${line.pendingQty > 0 ? 'text-orange-600' : 'text-green-600'}`}>{line.pendingQty}</span>
                    </td>
                    <td className="px-3 py-2.5 text-center"><QcBadge status={line.qcStatus} /></td>
                    <td className="px-3 py-2.5 text-right text-xs text-gray-600 font-mono">{formatCurrency(line.unitRate)}</td>
                    <td className="px-3 py-2.5 text-right text-xs text-gray-500">{line.taxPercent}%</td>
                    <td className="px-3 py-2.5 text-right text-xs font-medium text-gray-800">{formatCurrency(line.lineTotal)}</td>
                    <td className="px-3 py-2.5 text-center">
                      <div className="flex items-center justify-center gap-1">
                        <button type="button" title="Remarks" className="p-1 rounded hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors" onClick={() => { const note = prompt('Line remarks:', line.remarks); if (note !== null) updateLine(line.id, 'remarks', note); }}>
                          <span className="material-symbols-outlined text-[18px]">sticky_note_2</span>
                        </button>
                        <button type="button" title="Remove" className="p-1 rounded hover:bg-red-50 text-gray-400 hover:text-red-500 transition-colors" onClick={() => removeLine(line.id)}>
                          <span className="material-symbols-outlined text-[18px]">delete</span>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          <div className="mt-4 flex items-center justify-between">
            <div className="text-xs text-gray-400">Items are loaded from selected Purchase Orders</div>
            <div className="flex items-center gap-6 text-xs text-gray-500">
              <span>Items: <strong className="text-gray-800">{totalItems}</strong></span>
              <span>Received: <strong className="text-gray-800">{totalQty}</strong></span>
            </div>
          </div>
        </Section>

        {/* 5. FINANCIAL SUMMARY */}
        <Section icon="account_balance" title="Financial Summary" color="blue">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-5">
            <div className="rounded-xl bg-gray-50 p-4 text-center">
              <p className="text-[10px] font-bold uppercase tracking-tighter text-gray-400 mb-1">Basic Value</p>
              <p className="text-lg font-bold text-gray-800">{formatCurrency(basicValue)}</p>
            </div>
            <div className="rounded-xl bg-gray-50 p-4 text-center">
              <p className="text-[10px] font-bold uppercase tracking-tighter text-gray-400 mb-1">Tax Amount (GST)</p>
              <p className="text-lg font-bold text-gray-800">{formatCurrency(totalTax)}</p>
            </div>
            <div className="rounded-xl bg-gray-50 p-4 text-center">
              <p className="text-[10px] font-bold uppercase tracking-tighter text-gray-400 mb-1">Additional Charges</p>
              <p className="text-lg font-bold text-gray-800">{formatCurrency(0)}</p>
            </div>
            <div className="rounded-xl bg-blue-50 border border-blue-200 p-4 text-center">
              <p className="text-[10px] font-bold uppercase tracking-tighter text-blue-500 mb-1">Total GRN Value</p>
              <p className="text-lg font-bold text-blue-700">{formatCurrency(totalValue)}</p>
            </div>
          </div>
        </Section>

        {/* 6. LOGISTICS */}
        <Section icon="local_shipping" title="Logistics & Inward Details" color="amber">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
            <div>
              <FieldLabel required>Supplier Invoice No</FieldLabel>
              <input type="text" value={challanNo} onChange={(e) => setChallanNo(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel required>Invoice Date</FieldLabel>
              <input type="date" value={challanDate} onChange={(e) => setChallanDate(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>Gate Inward Entry</FieldLabel>
              <select value={gateEntry} onChange={(e) => setGateEntry(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent">
                <option value="">-- Select Entry --</option>
                <option value="GE-2026-03-0145">GE-2026-03-0145</option>
                <option value="GE-2026-03-0146">GE-2026-03-0146</option>
                <option value="GE-2026-03-0147">GE-2026-03-0147</option>
              </select>
            </div>
            <div>
              <FieldLabel>Gate Entry Date</FieldLabel>
              <input type="date" value={gateEntryDate} readOnly className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed" />
            </div>
            <div>
              <FieldLabel>Transporter Name</FieldLabel>
              <input type="text" value={transporterName} onChange={(e) => setTransporterName(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>Vehicle Number</FieldLabel>
              <input type="text" value={vehicleNumber} onChange={(e) => setVehicleNumber(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent uppercase" />
            </div>
            <div>
              <FieldLabel>Driver Name / Contact</FieldLabel>
              <input type="text" value={driverInfo} onChange={(e) => setDriverInfo(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>E-Way Bill Number</FieldLabel>
              <input type="text" value={ewayBill} onChange={(e) => setEwayBill(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
          </div>
        </Section>

        {/* 7. ATTACHMENTS & REMARKS */}
        <Section icon="attach_file" title="Attachments & Remarks" color="rose">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            <div>
              <FieldLabel>Attachments</FieldLabel>
              <div
                onDragOver={(e) => { e.preventDefault(); setIsDragging(true); }}
                onDragLeave={() => setIsDragging(false)}
                onDrop={(e) => { e.preventDefault(); setIsDragging(false); const dropped = Array.from(e.dataTransfer.files); setFiles(prev => [...prev, ...dropped.map(f => ({ name: f.name, size: `${(f.size / 1024 / 1024).toFixed(1)} MB` }))]); }}
                className={`border-2 border-dashed rounded-xl p-6 text-center transition-colors ${isDragging ? 'border-rose-400 bg-rose-50' : 'border-gray-200 hover:border-gray-300'}`}
              >
                <span className="material-symbols-outlined text-3xl text-gray-300 mb-2 block">cloud_upload</span>
                <p className="text-sm text-gray-500">Drag & drop files here, or</p>
                <label className="mt-2 inline-flex items-center gap-1 text-sm font-medium text-rose-600 hover:text-rose-700 cursor-pointer">
                  <span className="material-symbols-outlined text-base">upload_file</span>Browse Files
                  <input type="file" multiple className="hidden" onChange={(e) => { const sel = Array.from(e.target.files || []); setFiles(prev => [...prev, ...sel.map(f => ({ name: f.name, size: `${(f.size / 1024 / 1024).toFixed(1)} MB` }))]); }} />
                </label>
                <p className="text-[10px] text-gray-400 mt-2">PDF, JPG, PNG, ZIP up to 25 MB each</p>
              </div>
              {files.length > 0 && (
                <div className="mt-3 space-y-2">
                  {files.map((f, i) => (
                    <div key={i} className="flex items-center justify-between rounded-lg border border-gray-100 bg-gray-50 px-3 py-2">
                      <div className="flex items-center gap-2">
                        <span className="material-symbols-outlined text-base text-gray-400">description</span>
                        <span className="text-sm text-gray-700">{f.name}</span>
                        <span className="text-[10px] text-gray-400">{f.size}</span>
                      </div>
                      <button type="button" onClick={() => removeFile(i)} className="p-1 rounded hover:bg-red-50 text-gray-400 hover:text-red-500 transition-colors">
                        <span className="material-symbols-outlined text-base">close</span>
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>
            <div>
              <FieldLabel>Internal Notes / Remarks</FieldLabel>
              <textarea value={remarks} onChange={(e) => setRemarks(e.target.value)} rows={6} placeholder="Add any internal notes about this receipt..." className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-rose-500 focus:border-transparent resize-none" />
              <p className="text-[10px] text-gray-400 mt-1">{remarks.length} / 500 characters</p>
            </div>
          </div>
        </Section>
      </div>

      {/* FIXED FOOTER */}
      <div className="fixed bottom-0 left-0 right-0 z-40 bg-white border-t border-gray-200 shadow-[0_-4px_20px_rgba(0,0,0,0.06)]">
        <div className="border-b border-gray-100 bg-gray-50 px-6 py-2.5">
          <div className="max-w-screen-2xl mx-auto flex items-center justify-between text-xs text-gray-500">
            <div className="flex items-center gap-6">
              <span className="flex items-center gap-1.5">
                <span className="material-symbols-outlined text-sm text-blue-500">inventory_2</span>
                Items Received: <strong className="text-gray-800">{totalItems}</strong>
              </span>
              <span className="flex items-center gap-1.5">
                <span className="material-symbols-outlined text-sm text-emerald-500">scale</span>
                Total Quantity: <strong className="text-gray-800">{totalQty}</strong>
              </span>
            </div>
            <span className="flex items-center gap-1.5">
              <span className="material-symbols-outlined text-sm text-violet-500">payments</span>
              Total GRN Value: <strong className="text-gray-800 text-sm">{formatCurrency(totalValue)}</strong>
            </span>
          </div>
        </div>
        <div className="px-6 py-3">
          <div className="max-w-screen-2xl mx-auto flex items-center justify-between">
            <button type="button" className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 transition-colors">
              <span className="material-symbols-outlined text-lg">print</span>Print Preview
            </button>
            <div className="flex items-center gap-3">
              <Button variant="outline" onClick={() => router.push('/inventory/grn')}>Cancel</Button>
              <Button variant="secondary"><span className="material-symbols-outlined text-base mr-1.5">save</span>Save as Draft</Button>
              <Button><span className="material-symbols-outlined text-base mr-1.5">check_circle</span>Submit GRN</Button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
