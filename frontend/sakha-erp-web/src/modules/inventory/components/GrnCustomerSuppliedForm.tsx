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
  soNumber: string;
  itemName: string;
  itemDescription: string;
  itemCode: string;
  uom: string;
  receivedQty: number;
  qcStatus: 'Pending' | 'Passed' | 'Failed';
  unitRate: number;
  lineTotal: number;
  remarks: string;
}

/* ------------------------------------------------------------------ */
/*  Mock pre-filled data (matches the HTML wireframe)                  */
/* ------------------------------------------------------------------ */
const INITIAL_LINES: LineItem[] = [
  {
    id: '1',
    soNumber: 'SO-2026-0045',
    itemName: 'Woodfree Uncoated Paper 80 GSM',
    itemDescription: 'Premium woodfree uncoated paper, A4 size, 80 GSM, white',
    itemCode: 'RM-PAP-001',
    uom: 'Reams',
    receivedQty: 500,
    qcStatus: 'Pending',
    unitRate: 0,
    lineTotal: 0,
    remarks: '',
  },
  {
    id: '2',
    soNumber: 'SO-2026-0045',
    itemName: 'Soy-Based Offset Ink - Black',
    itemDescription: 'Eco-friendly soy-based offset printing ink, black, 5kg tin',
    itemCode: 'RM-INK-042',
    uom: 'Tins',
    receivedQty: 20,
    qcStatus: 'Passed',
    unitRate: 0,
    lineTotal: 0,
    remarks: '',
  },
  {
    id: '3',
    soNumber: 'SO-2026-0048',
    itemName: 'PUR Binding Adhesive',
    itemDescription: 'Polyurethane reactive binding adhesive, 20kg drum',
    itemCode: 'RM-ADH-015',
    uom: 'Drums',
    receivedQty: 5,
    qcStatus: 'Pending',
    unitRate: 0,
    lineTotal: 0,
    remarks: '',
  },
];

const ITEM_OPTIONS = [
  { value: 'RM-PAP-001', label: 'Woodfree Uncoated Paper 80 GSM', uom: 'Reams' },
  { value: 'RM-PAP-002', label: 'Art Paper 130 GSM - Gloss', uom: 'Reams' },
  { value: 'RM-INK-042', label: 'Soy-Based Offset Ink - Black', uom: 'Tins' },
  { value: 'RM-INK-043', label: 'Soy-Based Offset Ink - Cyan', uom: 'Tins' },
  { value: 'RM-ADH-015', label: 'PUR Binding Adhesive', uom: 'Drums' },
  { value: 'RM-THR-008', label: 'Book Binding Thread - Heavy Duty', uom: 'Spools' },
];

const CUSTOMER_CONTACTS = [
  { value: 'rahul', label: 'Rahul Mehta - Project Manager' },
  { value: 'anita', label: 'Anita Sharma - Procurement Lead' },
  { value: 'vikram', label: 'Vikram Patel - Logistics Head' },
];

/* ------------------------------------------------------------------ */
/*  Helper: Section wrapper                                            */
/* ------------------------------------------------------------------ */
function Section({ icon, title, children, color = 'blue' }: { icon: string; title: string; children: React.ReactNode; color?: 'blue' | 'emerald' | 'violet' | 'amber' | 'rose' }) {
  const colorMap = {
    blue: 'bg-blue-50 text-blue-600',
    emerald: 'bg-emerald-50 text-emerald-600',
    violet: 'bg-violet-50 text-violet-600',
    amber: 'bg-amber-50 text-amber-600',
    rose: 'bg-rose-50 text-rose-600',
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

/* ------------------------------------------------------------------ */
/*  Helper: Form field label                                           */
/* ------------------------------------------------------------------ */
function FieldLabel({ children, required }: { children: React.ReactNode; required?: boolean }) {
  return (
    <label className="block text-[11px] font-bold uppercase tracking-tighter text-gray-500 mb-1.5">
      {children}
      {required && <span className="text-red-500 ml-0.5">*</span>}
    </label>
  );
}

/* ------------------------------------------------------------------ */
/*  QC Badge                                                           */
/* ------------------------------------------------------------------ */
function QcBadge({ status }: { status: 'Pending' | 'Passed' | 'Failed' }) {
  const map = {
    Pending: 'bg-yellow-100 text-yellow-700',
    Passed: 'bg-green-100 text-green-700',
    Failed: 'bg-red-100 text-red-700',
  };
  const dotMap = {
    Pending: 'bg-yellow-500',
    Passed: 'bg-green-500',
    Failed: 'bg-red-500',
  };
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
export default function GrnCustomerSuppliedForm() {
  const router = useRouter();

  // ----- Header info state -----
  const [grnDate, setGrnDate] = useState('2026-03-31');
  const [warehouse, setWarehouse] = useState('WH-A');
  const [createdBy] = useState('Arjun Krishnamurthy');

  // ----- Customer state -----
  const [customerSearch, setCustomerSearch] = useState('Global Publishing Solutions');
  const [customerName] = useState('Global Publishing Solutions Pvt. Ltd.');
  const [customerGstin] = useState('27AABCG1234F1Z5');
  const [billingAddress] = useState('Plot 42, MIDC Industrial Area, Andheri East, Mumbai - 400093, Maharashtra');
  const [contactPerson, setContactPerson] = useState('rahul');
  const [contactEmail] = useState('rahul.mehta@globalpub.com');
  const [contactPhone] = useState('+91 98765 43210');

  // ----- Line items -----
  const [lineItems, setLineItems] = useState<LineItem[]>(INITIAL_LINES);

  // ----- Logistics state -----
  const [challanNo, setChallanNo] = useState('CSP/CH/2026/0892');
  const [challanDate, setChallanDate] = useState('2026-03-29');
  const [gateEntry, setGateEntry] = useState('GE-2026-03-0147');
  const [gateEntryDate, setGateEntryDate] = useState('2026-03-31');
  const [transporterName, setTransporterName] = useState('BlueDart Express');
  const [vehicleNumber, setVehicleNumber] = useState('MH 02 AB 1234');
  const [driverInfo, setDriverInfo] = useState('Rajesh Kumar / +91 87654 32100');
  const [ewayBill, setEwayBill] = useState('');

  // ----- Attachments & Remarks -----
  const [files, setFiles] = useState<{ name: string; size: string }[]>([
    { name: 'delivery_challan_0892.pdf', size: '2.4 MB' },
    { name: 'material_photos.zip', size: '8.1 MB' },
  ]);
  const [remarks, setRemarks] = useState('');
  const [isDragging, setIsDragging] = useState(false);

  // ----- Line item helpers -----
  const addLine = useCallback(() => {
    const newId = String(Date.now());
    setLineItems(prev => [
      ...prev,
      {
        id: newId,
        soNumber: '',
        itemName: '',
        itemDescription: '',
        itemCode: '',
        uom: '',
        receivedQty: 0,
        qcStatus: 'Pending',
        unitRate: 0,
        lineTotal: 0,
        remarks: '',
      },
    ]);
  }, []);

  const removeLine = useCallback((id: string) => {
    setLineItems(prev => prev.filter(l => l.id !== id));
  }, []);

  const updateLine = useCallback((id: string, field: keyof LineItem, value: any) => {
    setLineItems(prev =>
      prev.map(l => {
        if (l.id !== id) return l;
        const updated = { ...l, [field]: value };
        // Auto-fill item details on item selection
        if (field === 'itemCode') {
          const found = ITEM_OPTIONS.find(o => o.value === value);
          if (found) {
            updated.itemName = found.label;
            updated.uom = found.uom;
            updated.itemDescription = found.label;
          }
        }
        // Recalc total (for CSP it's 0 since unit rate is 0, but support entry)
        if (field === 'receivedQty' || field === 'unitRate') {
          updated.lineTotal = updated.receivedQty * updated.unitRate;
        }
        return updated;
      }),
    );
  }, []);

  const removeFile = useCallback((idx: number) => {
    setFiles(prev => prev.filter((_, i) => i !== idx));
  }, []);

  // ----- Computed -----
  const totalItems = lineItems.length;
  const totalQty = lineItems.reduce((s, l) => s + l.receivedQty, 0);
  const totalValue = lineItems.reduce((s, l) => s + l.lineTotal, 0);

  return (
    <>
      <PageHeader
        title="Create GRN - Customer Supplied"
        subtitle="Receive customer-supplied materials against sales orders"
        breadcrumbs={[
          { label: 'Inventory', href: '/inventory' },
          { label: 'Goods Receipt', href: '/inventory/grn' },
          { label: 'Create - Customer Supplied' },
        ]}
      />

      <div className="space-y-6 pb-32">
        {/* ============================================================ */}
        {/*  1. HEADER INFORMATION                                        */}
        {/* ============================================================ */}
        <Section icon="description" title="Header Information" color="blue">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
            {/* GRN Number */}
            <div>
              <FieldLabel>GRN Number</FieldLabel>
              <input
                type="text"
                value="GRN-2026-03-AUTO"
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed"
              />
              <p className="text-[10px] text-gray-400 mt-1">Auto-generated on save</p>
            </div>

            {/* GRN Date */}
            <div>
              <FieldLabel required>GRN Date</FieldLabel>
              <input
                type="date"
                value={grnDate}
                onChange={(e) => setGrnDate(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              />
            </div>

            {/* GRN Type */}
            <div>
              <FieldLabel>GRN Type</FieldLabel>
              <div className="flex items-center gap-2 rounded-md border border-blue-200 bg-blue-50 px-3 py-2">
                <span className="material-symbols-outlined text-blue-600 text-lg">person</span>
                <span className="text-sm font-medium text-blue-700">Customer Supplied</span>
              </div>
            </div>

            {/* Status */}
            <div>
              <FieldLabel>Status</FieldLabel>
              <span className="inline-flex items-center gap-1.5 rounded-full bg-gray-100 px-3 py-2 text-xs font-medium text-gray-600">
                <span className="h-2 w-2 rounded-full bg-gray-400" />
                Draft
              </span>
            </div>

            {/* Site */}
            <div>
              <FieldLabel>Site</FieldLabel>
              <input
                type="text"
                value="Main Manufacturing Unit - Pune"
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed"
              />
            </div>

            {/* Warehouse */}
            <div>
              <FieldLabel required>Receiving Warehouse</FieldLabel>
              <select
                value={warehouse}
                onChange={(e) => setWarehouse(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
              >
                <option value="WH-A">Warehouse A - Raw Materials</option>
                <option value="WH-B">Warehouse B - Components</option>
                <option value="WH-C">Warehouse C - Consumables</option>
              </select>
            </div>

            {/* Created By */}
            <div>
              <FieldLabel>Created By</FieldLabel>
              <div className="flex items-center gap-2 rounded-md border border-gray-200 bg-gray-50 px-3 py-2">
                <div className="h-6 w-6 rounded-full bg-blue-100 flex items-center justify-center text-xs font-bold text-blue-600">AK</div>
                <span className="text-sm text-gray-700">{createdBy}</span>
              </div>
            </div>
          </div>
        </Section>

        {/* ============================================================ */}
        {/*  2. CUSTOMER INFORMATION                                      */}
        {/* ============================================================ */}
        <Section icon="person_search" title="Customer Information" color="emerald">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
            {/* Search Customer */}
            <div className="lg:col-span-2">
              <FieldLabel required>Search Customer</FieldLabel>
              <div className="relative">
                <span className="material-symbols-outlined absolute left-3 top-1/2 -translate-y-1/2 text-gray-400 text-lg">search</span>
                <input
                  type="text"
                  value={customerSearch}
                  onChange={(e) => setCustomerSearch(e.target.value)}
                  placeholder="Type customer name, code or GSTIN..."
                  className="block w-full rounded-md border border-gray-300 pl-10 pr-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent"
                />
              </div>
            </div>

            {/* GSTIN */}
            <div>
              <FieldLabel>GSTIN</FieldLabel>
              <input
                type="text"
                value={customerGstin}
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed font-mono"
              />
            </div>

            {/* Customer Name */}
            <div className="lg:col-span-2">
              <FieldLabel>Customer Name</FieldLabel>
              <input
                type="text"
                value={customerName}
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-700 cursor-not-allowed"
              />
            </div>

            {/* Billing Address */}
            <div className="lg:col-span-3">
              <FieldLabel>Billing Address</FieldLabel>
              <textarea
                value={billingAddress}
                readOnly
                rows={2}
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed resize-none"
              />
            </div>

            {/* Contact Person */}
            <div>
              <FieldLabel>Contact Person</FieldLabel>
              <select
                value={contactPerson}
                onChange={(e) => setContactPerson(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-emerald-500 focus:border-transparent"
              >
                {CUSTOMER_CONTACTS.map(c => (
                  <option key={c.value} value={c.value}>{c.label}</option>
                ))}
              </select>
            </div>

            {/* Email */}
            <div>
              <FieldLabel>Email</FieldLabel>
              <input
                type="email"
                value={contactEmail}
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed"
              />
            </div>

            {/* Phone */}
            <div>
              <FieldLabel>Phone</FieldLabel>
              <input
                type="text"
                value={contactPhone}
                readOnly
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-600 cursor-not-allowed"
              />
            </div>
          </div>
        </Section>

        {/* ============================================================ */}
        {/*  3. LINE ITEMS TABLE                                          */}
        {/* ============================================================ */}
        <Section icon="list_alt" title="Line Items - Material Details" color="violet">
          <div className="overflow-x-auto -mx-5">
            <table className="w-full text-sm min-w-[1100px]">
              <thead>
                <tr className="bg-gray-50 border-y border-gray-200">
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-10">#</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">SO Ref</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500">Item</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">Item Code</th>
                  <th className="text-left px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-20">UOM</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Recv Qty</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">QC Status</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-24">Unit Rate</th>
                  <th className="text-right px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-28">Line Total</th>
                  <th className="text-center px-3 py-2.5 text-[11px] font-bold uppercase tracking-tighter text-gray-500 w-16">Actions</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map((line, idx) => (
                  <tr key={line.id} className={`border-b border-gray-100 ${idx % 2 === 1 ? 'bg-gray-50/50' : 'bg-white'} hover:bg-blue-50/30 transition-colors`}>
                    {/* Row number */}
                    <td className="px-3 py-2.5 text-gray-400 font-medium">{idx + 1}</td>

                    {/* SO Ref */}
                    <td className="px-3 py-2.5">
                      <input
                        type="text"
                        value={line.soNumber}
                        onChange={(e) => updateLine(line.id, 'soNumber', e.target.value)}
                        placeholder="SO #"
                        className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-violet-400 focus:border-transparent bg-transparent"
                      />
                    </td>

                    {/* Item */}
                    <td className="px-3 py-2.5">
                      <select
                        value={line.itemCode}
                        onChange={(e) => updateLine(line.id, 'itemCode', e.target.value)}
                        className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs focus:outline-none focus:ring-1 focus:ring-violet-400 focus:border-transparent bg-transparent"
                      >
                        <option value="">-- Select Item --</option>
                        {ITEM_OPTIONS.map(o => (
                          <option key={o.value} value={o.value}>{o.label}</option>
                        ))}
                      </select>
                      {line.itemDescription && (
                        <p className="text-[10px] text-gray-400 mt-0.5 truncate">{line.itemDescription}</p>
                      )}
                    </td>

                    {/* Item Code */}
                    <td className="px-3 py-2.5">
                      <span className="text-xs font-mono text-gray-600">{line.itemCode || '-'}</span>
                    </td>

                    {/* UOM */}
                    <td className="px-3 py-2.5">
                      <span className="text-xs text-gray-600">{line.uom || '-'}</span>
                    </td>

                    {/* Received Qty */}
                    <td className="px-3 py-2.5">
                      <input
                        type="number"
                        value={line.receivedQty || ''}
                        onChange={(e) => updateLine(line.id, 'receivedQty', Number(e.target.value))}
                        min={0}
                        className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 focus:border-transparent bg-transparent"
                      />
                    </td>

                    {/* QC Status */}
                    <td className="px-3 py-2.5 text-center">
                      <QcBadge status={line.qcStatus} />
                    </td>

                    {/* Unit Rate */}
                    <td className="px-3 py-2.5">
                      <input
                        type="number"
                        value={line.unitRate || ''}
                        onChange={(e) => updateLine(line.id, 'unitRate', Number(e.target.value))}
                        min={0}
                        step={0.01}
                        placeholder="0.00"
                        className="w-full rounded-md border border-gray-200 px-2 py-1.5 text-xs text-right focus:outline-none focus:ring-1 focus:ring-violet-400 focus:border-transparent bg-transparent text-gray-400"
                      />
                    </td>

                    {/* Line Total */}
                    <td className="px-3 py-2.5 text-right">
                      <span className="text-xs font-medium text-gray-700">
                        {line.lineTotal > 0 ? formatCurrency(line.lineTotal) : <span className="text-gray-300">--</span>}
                      </span>
                    </td>

                    {/* Actions */}
                    <td className="px-3 py-2.5 text-center">
                      <div className="flex items-center justify-center gap-1">
                        <button
                          type="button"
                          title="Remarks"
                          className="p-1 rounded hover:bg-gray-100 text-gray-400 hover:text-gray-600 transition-colors"
                          onClick={() => {
                            const note = prompt('Line remarks:', line.remarks);
                            if (note !== null) updateLine(line.id, 'remarks', note);
                          }}
                        >
                          <span className="material-symbols-outlined text-[18px]">sticky_note_2</span>
                        </button>
                        <button
                          type="button"
                          title="Remove line"
                          className="p-1 rounded hover:bg-red-50 text-gray-400 hover:text-red-500 transition-colors"
                          onClick={() => removeLine(line.id)}
                        >
                          <span className="material-symbols-outlined text-[18px]">delete</span>
                        </button>
                      </div>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>

          {/* Add Line Button */}
          <div className="mt-4 flex items-center justify-between">
            <button
              type="button"
              onClick={addLine}
              className="inline-flex items-center gap-1.5 text-sm font-medium text-violet-600 hover:text-violet-700 transition-colors"
            >
              <span className="material-symbols-outlined text-lg">add_circle</span>
              Add Line Item
            </button>

            {/* Summary */}
            <div className="flex items-center gap-6 text-xs text-gray-500">
              <span>Total Items: <strong className="text-gray-800">{totalItems}</strong></span>
              <span>Total Received Qty: <strong className="text-gray-800">{totalQty}</strong></span>
            </div>
          </div>
        </Section>

        {/* ============================================================ */}
        {/*  4. LOGISTICS & INWARD DETAILS                                */}
        {/* ============================================================ */}
        <Section icon="local_shipping" title="Logistics & Inward Details" color="amber">
          <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-5">
            {/* Supplier Challan No */}
            <div>
              <FieldLabel required>Supplier Challan No</FieldLabel>
              <input
                type="text"
                value={challanNo}
                onChange={(e) => setChallanNo(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>

            {/* Challan Date */}
            <div>
              <FieldLabel required>Challan Date</FieldLabel>
              <input
                type="date"
                value={challanDate}
                onChange={(e) => setChallanDate(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>

            {/* Gate Inward Entry */}
            <div>
              <FieldLabel>Gate Inward Entry</FieldLabel>
              <select
                value={gateEntry}
                onChange={(e) => setGateEntry(e.target.value)}
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              >
                <option value="">-- Select Entry --</option>
                <option value="GE-2026-03-0145">GE-2026-03-0145</option>
                <option value="GE-2026-03-0146">GE-2026-03-0146</option>
                <option value="GE-2026-03-0147">GE-2026-03-0147</option>
              </select>
            </div>

            {/* Gate Entry Date */}
            <div>
              <FieldLabel>Gate Entry Date</FieldLabel>
              <input
                type="date"
                value={gateEntryDate}
                onChange={(e) => setGateEntryDate(e.target.value)}
                className="block w-full rounded-md border border-gray-200 bg-gray-50 px-3 py-2 text-sm text-gray-500 cursor-not-allowed"
                readOnly
              />
            </div>

            {/* Transporter Name */}
            <div>
              <FieldLabel>Transporter Name</FieldLabel>
              <input
                type="text"
                value={transporterName}
                onChange={(e) => setTransporterName(e.target.value)}
                placeholder="Enter transporter name"
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>

            {/* Vehicle Number */}
            <div>
              <FieldLabel>Vehicle Number</FieldLabel>
              <input
                type="text"
                value={vehicleNumber}
                onChange={(e) => setVehicleNumber(e.target.value)}
                placeholder="MH 00 XX 0000"
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent uppercase"
              />
            </div>

            {/* Driver Name/Contact */}
            <div>
              <FieldLabel>Driver Name / Contact</FieldLabel>
              <input
                type="text"
                value={driverInfo}
                onChange={(e) => setDriverInfo(e.target.value)}
                placeholder="Name / Phone"
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>

            {/* E-Way Bill Number */}
            <div>
              <FieldLabel>E-Way Bill Number</FieldLabel>
              <input
                type="text"
                value={ewayBill}
                onChange={(e) => setEwayBill(e.target.value)}
                placeholder="Optional"
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-amber-500 focus:border-transparent"
              />
            </div>
          </div>
        </Section>

        {/* ============================================================ */}
        {/*  5. ATTACHMENTS & REMARKS                                     */}
        {/* ============================================================ */}
        <Section icon="attach_file" title="Attachments & Remarks" color="rose">
          <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
            {/* Attachments */}
            <div>
              <FieldLabel>Attachments</FieldLabel>
              {/* Drop zone */}
              <div
                onDragOver={(e) => { e.preventDefault(); setIsDragging(true); }}
                onDragLeave={() => setIsDragging(false)}
                onDrop={(e) => {
                  e.preventDefault();
                  setIsDragging(false);
                  const dropped = Array.from(e.dataTransfer.files);
                  setFiles(prev => [
                    ...prev,
                    ...dropped.map(f => ({ name: f.name, size: `${(f.size / 1024 / 1024).toFixed(1)} MB` })),
                  ]);
                }}
                className={`border-2 border-dashed rounded-xl p-6 text-center transition-colors ${
                  isDragging ? 'border-rose-400 bg-rose-50' : 'border-gray-200 hover:border-gray-300'
                }`}
              >
                <span className="material-symbols-outlined text-3xl text-gray-300 mb-2 block">cloud_upload</span>
                <p className="text-sm text-gray-500">Drag & drop files here, or</p>
                <label className="mt-2 inline-flex items-center gap-1 text-sm font-medium text-rose-600 hover:text-rose-700 cursor-pointer">
                  <span className="material-symbols-outlined text-base">upload_file</span>
                  Browse Files
                  <input
                    type="file"
                    multiple
                    className="hidden"
                    onChange={(e) => {
                      const selected = Array.from(e.target.files || []);
                      setFiles(prev => [
                        ...prev,
                        ...selected.map(f => ({ name: f.name, size: `${(f.size / 1024 / 1024).toFixed(1)} MB` })),
                      ]);
                    }}
                  />
                </label>
                <p className="text-[10px] text-gray-400 mt-2">PDF, JPG, PNG, ZIP up to 25 MB each</p>
              </div>

              {/* File list */}
              {files.length > 0 && (
                <div className="mt-3 space-y-2">
                  {files.map((f, i) => (
                    <div key={i} className="flex items-center justify-between rounded-lg border border-gray-100 bg-gray-50 px-3 py-2">
                      <div className="flex items-center gap-2">
                        <span className="material-symbols-outlined text-base text-gray-400">description</span>
                        <span className="text-sm text-gray-700">{f.name}</span>
                        <span className="text-[10px] text-gray-400">{f.size}</span>
                      </div>
                      <button
                        type="button"
                        onClick={() => removeFile(i)}
                        className="p-1 rounded hover:bg-red-50 text-gray-400 hover:text-red-500 transition-colors"
                      >
                        <span className="material-symbols-outlined text-base">close</span>
                      </button>
                    </div>
                  ))}
                </div>
              )}
            </div>

            {/* Remarks */}
            <div>
              <FieldLabel>Internal Notes / Remarks</FieldLabel>
              <textarea
                value={remarks}
                onChange={(e) => setRemarks(e.target.value)}
                rows={6}
                placeholder="Add any internal notes, special instructions, or observations about this receipt..."
                className="block w-full rounded-md border border-gray-300 px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-rose-500 focus:border-transparent resize-none"
              />
              <p className="text-[10px] text-gray-400 mt-1">{remarks.length} / 500 characters</p>
            </div>
          </div>
        </Section>
      </div>

      {/* ============================================================ */}
      {/*  6 + 7. FIXED FOOTER — Actions + Summary Bar                   */}
      {/* ============================================================ */}
      <div className="fixed bottom-0 left-0 right-0 z-40 bg-white border-t border-gray-200 shadow-[0_-4px_20px_rgba(0,0,0,0.06)]">
        {/* Summary bar */}
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

        {/* Action buttons */}
        <div className="px-6 py-3">
          <div className="max-w-screen-2xl mx-auto flex items-center justify-between">
            <button
              type="button"
              className="inline-flex items-center gap-1.5 text-sm text-gray-500 hover:text-gray-700 transition-colors"
            >
              <span className="material-symbols-outlined text-lg">print</span>
              Print Preview
            </button>

            <div className="flex items-center gap-3">
              <Button
                variant="outline"
                onClick={() => router.push('/inventory/grn')}
              >
                Cancel
              </Button>
              <Button variant="secondary">
                <span className="material-symbols-outlined text-base mr-1.5">save</span>
                Save as Draft
              </Button>
              <Button>
                <span className="material-symbols-outlined text-base mr-1.5">check_circle</span>
                Submit GRN
              </Button>
            </div>
          </div>
        </div>
      </div>
    </>
  );
}
