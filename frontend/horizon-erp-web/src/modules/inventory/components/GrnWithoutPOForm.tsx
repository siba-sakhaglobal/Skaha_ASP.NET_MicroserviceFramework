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
  itemName: string;
  itemDescription: string;
  itemCode: string;
  uom: string;
  receivedQty: number;
  qcStatus: 'Pending' | 'Passed' | 'Failed';
  unitRate: number;
  lastPurchasePrice: number;
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
    id: '1', itemName: 'Nylon Cable Tie 300mm', itemDescription: 'Heavy duty nylon cable tie, 300mm, UV resistant, black',
    itemCode: 'CN-CBL-019', uom: 'Packs', receivedQty: 100, qcStatus: 'Pending',
    unitRate: 85, lastPurchasePrice: 82, taxPercent: 18, taxAmount: 1530, lineTotal: 10030, remarks: '',
  },
  {
    id: '2', itemName: 'WD-40 Multi-Use Spray 400ml', itemDescription: 'WD-40 multi-use product, 400ml aerosol can',
    itemCode: 'MN-SPR-003', uom: 'Cans', receivedQty: 24, qcStatus: 'Passed',
    unitRate: 320, lastPurchasePrice: 310, taxPercent: 18, taxAmount: 1382.4, lineTotal: 9062.4, remarks: '',
  },
];

const ITEM_OPTIONS = [
  { value: 'CN-CBL-019', label: 'Nylon Cable Tie 300mm', uom: 'Packs', lpp: 82 },
  { value: 'MN-SPR-003', label: 'WD-40 Multi-Use Spray 400ml', uom: 'Cans', lpp: 310 },
  { value: 'CN-TAP-007', label: 'Electrical Insulation Tape', uom: 'Rolls', lpp: 45 },
  { value: 'MN-GRE-011', label: 'Lithium Grease Cartridge', uom: 'Pcs', lpp: 180 },
  { value: 'HW-SCR-022', label: 'Self-Tapping Screw #10x1"', uom: 'Boxes', lpp: 220 },
];

/* ------------------------------------------------------------------ */
/*  Helpers                                                            */
/* ------------------------------------------------------------------ */
function Section({ icon, title, children, color = 'blue' }: { icon: string; title: string; children: React.ReactNode; color?: string }) {
  const colorMap: Record<string, string> = {
    blue: 'bg-blue-50 text-blue-600', emerald: 'bg-emerald-50 text-emerald-600', violet: 'bg-violet-50 text-violet-600',
    amber: 'bg-amber-50 text-amber-600', rose: 'bg-rose-50 text-rose-600', orange: 'bg-orange-50 text-orange-600',
  };
  return (
    <div className="bg-white rounded-2xl border border-gray-200 overflow-hidden">
      <div className={`flex items-center gap-2.5 px-5 py-3.5 border-b border-gray-100 ${colorMap[color] || colorMap.blue}`}>
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
      <span className={`h-1.5 w-1.5 rounded-full ${dotMap[status]}`} />{status}
    </span>
  );
}

/* ================================================================== */
/*  MAIN COMPONENT                                                     */
/* ================================================================== */
export default function GrnWithoutPOForm() {
  const router = useRouter();

  // Header
  const [grnDate, setGrnDate] = useState('2026-03-31');
  const [warehouse, setWarehouse] = useState('WH-C');
  const [createdBy] = useState('Arjun Krishnamurthy');

  // Vendor
  const [vendorSearch, setVendorSearch] = useState('Metro Hardware');
  const [vendorName] = useState('Metro Hardware & Industrial Supplies');
  const [vendorGstin] = useState('27AABCM5678D1Z3');
  const [vendorAddress] = useState('Shop 14, Industrial Supply Market, Pimpri-Chinchwad, Pune - 411018, Maharashtra');

  // Receipt Reason
  const [receiptReason, setReceiptReason] = useState('Urgent requirement of consumables for maintenance shutdown scheduled on 01-Apr-2026. Regular PO process could not be followed due to time constraints.');

  // Line Items
  const [lineItems, setLineItems] = useState<LineItem[]>(INITIAL_LINES);

  // Logistics
  const [challanNo, setChallanNo] = useState('MH/DC/2026/1184');
  const [challanDate, setChallanDate] = useState('2026-03-31');
  const [gateEntry, setGateEntry] = useState('GE-2026-03-0149');
  const [gateEntryDate] = useState('2026-03-31');
  const [transporterName, setTransporterName] = useState('Self-delivery');
  const [vehicleNumber, setVehicleNumber] = useState('MH 14 EF 9012');
  const [driverInfo, setDriverInfo] = useState('');
  const [ewayBill, setEwayBill] = useState('');

  // Attachments & Remarks
  const [files, setFiles] = useState<{ name: string; size: string }[]>([]);
  const [remarks, setRemarks] = useState('');
  const [isDragging, setIsDragging] = useState(false);

  const addLine = useCallback(() => {
    setLineItems(prev => [...prev, {
      id: String(Date.now()), itemName: '', itemDescription: '', itemCode: '', uom: '',
      receivedQty: 0, qcStatus: 'Pending', unitRate: 0, lastPurchasePrice: 0,
      taxPercent: 18, taxAmount: 0, lineTotal: 0, remarks: '',
    }]);
  }, []);

  const removeLine = useCallback((id: string) => {
    setLineItems(prev => prev.filter(l => l.id !== id));
  }, []);

  const updateLine = useCallback((id: string, field: keyof LineItem, value: any) => {
    setLineItems(prev => prev.map(l => {
      if (l.id !== id) return l;
      const updated = { ...l, [field]: value };
      if (field === 'itemCode') {
        const found = ITEM_OPTIONS.find(o => o.value === value);
        if (found) {
          updated.itemName = found.label;
          updated.itemDescription = found.label;
          updated.uom = found.uom;
          updated.lastPurchasePrice = found.lpp;
        }
      }
      if (field === 'receivedQty' || field === 'unitRate' || field === 'taxPercent') {
        const base = updated.receivedQty * updated.unitRate;
        updated.taxAmount = base * (updated.taxPercent / 100);
        updated.lineTotal = base + updated.taxAmount;
      }
      return updated;
    }));
  }, []);

  const removeFile = useCallback((idx: number) => setFiles(prev => prev.filter((_, i) => i !== idx)), []);

  // Computed
  const totalItems = lineItems.length;
  const totalQty = lineItems.reduce((s, l) => s + l.receivedQty, 0);
  const basicValue = lineItems.reduce((s, l) => s + l.receivedQty * l.unitRate, 0);
  const totalTax = lineItems.reduce((s, l) => s + l.taxAmount, 0);
  const totalValue = lineItems.reduce((s, l) => s + l.lineTotal, 0);

  return (
    <>
      <PageHeader
        title="Create GRN - Without Purchase Order"
        subtitle="Receive goods without a purchase order reference"
        breadcrumbs={[
          { label: 'Inventory', href: '/inventory' },
          { label: 'Goods Receipt', href: '/inventory/grn' },
          { label: 'Create - Without PO' },
        ]}
      />

      <div className="space-y-6 pb-32">
        {/* 1. HEADER */}
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
                <span className="material-symbols-outlined text-blue-600 text-lg">receipt</span>
                <span className="text-sm font-medium text-blue-700">Without PO</span>
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

        {/* 2. VENDOR */}
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

        {/* 3. RECEIPT REASON */}
        <Section icon="help_outline" title="Receipt Reason (Mandatory)" color="orange">
          <div>
            <FieldLabel required>Reason for Receipt Without PO</FieldLabel>
            <textarea
              value={receiptReason}
              onChange={(e) => setReceiptReason(e.target.value)}
              rows={3}
              minLength={10}
              placeholder="Explain why this receipt is being processed without a Purchase Order (minimum 10 characters)..."
              className={`block w-full rounded-md border px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-orange-500 focus:border-transparent resize-none ${
                receiptReason.length < 10 ? 'border-red-300 bg-red-50/30' : 'border-gray-300'
              }`}
            />
            <div className="flex items-center justify-between mt-1">
              {receiptReason.length < 10 && (
                <p className="text-[10px] text-red-500">Minimum 10 characters required</p>
              )}
              <p className="text-[10px] text-gray-400 ml-auto">{receiptReason.length} characters</p>
            </div>
          </div>
        </Section>

        {/* 4. LINE ITEMS */}
        <Section icon="list_alt" title="Line Items - Manual Entry" color="violet">
          <div className="overflow-x-auto -mx-5">
            <table className="w-full text-sm min-w-[1200px]">
              <thead>
                <tr className="bg-gray-50 border-y border-gray-200">
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-10">#</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500">Item</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Code</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">UOM</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Recv Qty</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">QC</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Unit Rate</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">LPP</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-16">Tax %</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">Line Total</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-16">Actions</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map((line, idx) => (
                  <tr key={line.id} className={`border-b border-gray-100 ${idx % 2 === 1 ? 'bg-gray-50/50' : 'bg-white'} hover:bg-blue-50/30 transition-colors`}>
                    <td className="px-3 py-2.5 text-gray-400 font-medium">{idx + 1}</td>
                    <td className="px-3 py-2.5">
                      <select value={line.itemCode} onChange={(e) => updateLine(line.id, 'itemCode', e.target.value)} className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-violet-400 bg-transparent">
                        <option value="">-- Select Item --</option>
                        {ITEM_OPTIONS.map(o => <option key={o.value} value={o.value}>{o.label}</option>)}
                      </select>
                    </td>
                    <td className="px-3 py-2.5 text-xs font-mono text-gray-600">{line.itemCode || '-'}</td>
                    <td className="px-3 py-2.5 text-xs text-gray-600">{line.uom || '-'}</td>
                    <td className="px-3 py-2.5">
                      <input type="number" value={line.receivedQty || ''} onChange={(e) => updateLine(line.id, 'receivedQty', Number(e.target.value))} min={0} className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 bg-transparent" />
                    </td>
                    <td className="px-3 py-2.5 text-center"><QcBadge status={line.qcStatus} /></td>
                    <td className="px-3 py-2.5">
                      <input type="number" value={line.unitRate || ''} onChange={(e) => updateLine(line.id, 'unitRate', Number(e.target.value))} min={0} step={0.01} className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 bg-transparent" />
                    </td>
                    <td className="px-3 py-2.5 text-right">
                      {line.lastPurchasePrice > 0 && (
                        <span className="text-[10px] text-gray-400" title="Last Purchase Price">{formatCurrency(line.lastPurchasePrice)}</span>
                      )}
                    </td>
                    <td className="px-3 py-2.5">
                      <input type="number" value={line.taxPercent} onChange={(e) => updateLine(line.id, 'taxPercent', Number(e.target.value))} min={0} max={100} className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 bg-transparent" />
                    </td>
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
            <button type="button" onClick={addLine} className="inline-flex items-center gap-1.5 text-sm font-medium text-violet-600 hover:text-violet-700 transition-colors">
              <span className="material-symbols-outlined text-lg">add_circle</span>Add Line Item
            </button>
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
              <FieldLabel required>Delivery Challan No</FieldLabel>
              <input type="text" value={challanNo} onChange={(e) => setChallanNo(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel required>Challan Date</FieldLabel>
              <input type="date" value={challanDate} onChange={(e) => setChallanDate(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>Gate Inward Entry</FieldLabel>
              <select value={gateEntry} onChange={(e) => setGateEntry(e.target.value)} className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent">
                <option value="">-- Select Entry --</option>
                <option value="GE-2026-03-0148">GE-2026-03-0148</option>
                <option value="GE-2026-03-0149">GE-2026-03-0149</option>
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
              <input type="text" value={driverInfo} onChange={(e) => setDriverInfo(e.target.value)} placeholder="Name / Phone" className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
            </div>
            <div>
              <FieldLabel>E-Way Bill Number</FieldLabel>
              <input type="text" value={ewayBill} onChange={(e) => setEwayBill(e.target.value)} placeholder="Optional" className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent" />
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
