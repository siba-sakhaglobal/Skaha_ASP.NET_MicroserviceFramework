'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { PageHeader } from '@/components/layout/PageHeader';
import { Button } from '@/components/ui/Button';

/* ------------------------------------------------------------------ */
/*  Types                                                               */
/* ------------------------------------------------------------------ */
interface DcReference {
  id: string;
  dcNumber: string;
  dcDate: string;
  customerVendor: string;
  purpose: 'JOB WORK' | 'REPAIR';
  expectedReturn: string;
  status: 'PARTIALLY RETURNED' | 'PENDING RETURN';
}

interface LineItem {
  id: string;
  materialDescription: string;
  puom: string;
  dispatched: number;
  received: number;
  pending: number;
  qtyToRecv: number;
  qcStatus: 'Passed' | 'Pending' | 'Failed';
  freight: string;
  remarks: string;
}

/* ------------------------------------------------------------------ */
/*  Mock data                                                           */
/* ------------------------------------------------------------------ */
const DC_REFERENCES: DcReference[] = [
  {
    id: '1',
    dcNumber: 'DC/23/9012',
    dcDate: '15/01/2024',
    customerVendor: 'Precision Engineering Pvt Ltd',
    purpose: 'JOB WORK',
    expectedReturn: '10/02/2024',
    status: 'PARTIALLY RETURNED',
  },
  {
    id: '2',
    dcNumber: 'DC/23/9455',
    dcDate: '20/01/2024',
    customerVendor: 'Modern Forge Ltd',
    purpose: 'REPAIR',
    expectedReturn: '05/02/2024',
    status: 'PENDING RETURN',
  },
];

const INITIAL_LINE_ITEMS: LineItem[] = [
  {
    id: '1',
    materialDescription: 'Steel Rods 12mm - Grade A',
    puom: 'MT',
    dispatched: 10.0,
    received: 2.0,
    pending: 8.0,
    qtyToRecv: 8,
    qcStatus: 'Passed',
    freight: '₹1,200',
    remarks: '',
  },
  {
    id: '2',
    materialDescription: 'Cement Portland - High Strength',
    puom: 'BAGS',
    dispatched: 500.0,
    received: 0.0,
    pending: 500.0,
    qtyToRecv: 500,
    qcStatus: 'Pending',
    freight: '₹4,500',
    remarks: '',
  },
];

/* ------------------------------------------------------------------ */
/*  Component                                                           */
/* ------------------------------------------------------------------ */
export default function GrnReturnableDCForm() {
  const router = useRouter();

  const [selectedDcId, setSelectedDcId] = useState<string | null>(null);
  const [lineItems, setLineItems] = useState<LineItem[]>(INITIAL_LINE_ITEMS);

  // Logistics fields
  const [logistics, setLogistics] = useState({
    supplierChallanNo: '',
    challanDate: '',
    gateInwardNo: '',
    transporter: '',
    vehicleNumber: '',
    driverContact: '',
  });

  // Config fields
  const [config, setConfig] = useState({
    site: '',
    warehouse: '',
    chargesStructure: '',
  });

  const [remarks, setRemarks] = useState('');

  const handleQtyChange = (id: string, value: string) => {
    setLineItems((prev) =>
      prev.map((item) =>
        item.id === id ? { ...item, qtyToRecv: Number(value) } : item
      )
    );
  };

  const handleQcChange = (id: string, value: string) => {
    setLineItems((prev) =>
      prev.map((item) =>
        item.id === id
          ? { ...item, qcStatus: value as LineItem['qcStatus'] }
          : item
      )
    );
  };

  const handleRemarksChange = (id: string, value: string) => {
    setLineItems((prev) =>
      prev.map((item) =>
        item.id === id ? { ...item, remarks: value } : item
      )
    );
  };

  const qcBg = (status: LineItem['qcStatus']) => {
    if (status === 'Passed') return 'bg-green-50 text-green-700';
    if (status === 'Failed') return 'bg-red-50 text-red-700';
    return 'bg-yellow-50 text-yellow-700';
  };

  const inputCls =
    'bg-white border border-gray-200 rounded-lg px-4 py-2.5 text-sm outline-none focus:ring-2 focus:ring-blue-100 w-full';
  const readonlyCls =
    'bg-gray-50 border border-gray-200 rounded-lg px-4 py-2.5 text-sm outline-none w-full';
  const labelCls = 'text-[11px] font-bold text-gray-500 mb-1 block';

  return (
    <div className="min-h-screen bg-gray-50 pb-32">
      <PageHeader
        title="GRN – Returnable DC"
        subtitle="Record material return against a dispatched Returnable DC"
      />

      <div className="max-w-screen-xl mx-auto px-6 py-8 space-y-6">

        {/* ── Section 1: Summary Bento ─────────────────────────────── */}
        <div className="grid grid-cols-4 gap-4">
          {/* GRN Header Information — 3 cols */}
          <div className="col-span-3 bg-gray-50 border border-gray-200 p-6 rounded-xl">
            <p className="text-sm font-bold tracking-tight text-gray-900 mb-4 flex items-center gap-2">
              <span className="material-symbols-outlined text-base text-blue-600">receipt_long</span>
              GRN Header Information
            </p>
            <div className="grid grid-cols-5 gap-4">
              <div>
                <label className={labelCls}>GRN NUMBER</label>
                <input readOnly value="GRN/24/00312" className={readonlyCls} />
              </div>
              <div>
                <label className={labelCls}>GRN DATE</label>
                <input readOnly value="31/03/2024" className={readonlyCls} />
              </div>
              <div>
                <label className={labelCls}>GRN TYPE</label>
                <input readOnly value="Returnable DC" className={readonlyCls} />
              </div>
              <div>
                <label className={labelCls}>STATUS</label>
                <input readOnly value="Draft" className={readonlyCls} />
              </div>
              <div>
                <label className={labelCls}>CREATED BY</label>
                <input readOnly value="Rahul Sharma" className={readonlyCls} />
              </div>
            </div>
          </div>

          {/* Status card — 1 col */}
          <div className="col-span-1 bg-white border border-gray-200 p-6 rounded-xl flex flex-col items-center justify-center text-center gap-3">
            <div className="flex items-center gap-2">
              <span className="relative flex h-3 w-3">
                <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-yellow-400 opacity-75"></span>
                <span className="relative inline-flex rounded-full h-3 w-3 bg-yellow-400"></span>
              </span>
              <span className="text-2xl font-bold text-gray-800">Draft</span>
            </div>
            <p className="text-xs text-gray-400">Last edited 2 mins ago</p>
          </div>
        </div>

        {/* ── Section 2: Configuration Settings ───────────────────── */}
        <div className="bg-gray-50 border border-gray-200 p-8 rounded-xl">
          <p className="text-sm font-bold tracking-tight text-gray-900 mb-6 flex items-center gap-2">
            <span className="material-symbols-outlined text-base text-blue-600">settings</span>
            Configuration Settings
          </p>
          <div className="grid grid-cols-3 gap-6">
            <div>
              <label className={labelCls}>SELECT SITE</label>
              <select
                value={config.site}
                onChange={(e) => setConfig({ ...config, site: e.target.value })}
                className={inputCls}
              >
                <option value="">— Select Site —</option>
                <option value="site1">Site 1 – Main Plant</option>
                <option value="site2">Site 2 – North Wing</option>
              </select>
            </div>
            <div>
              <label className={labelCls}>SELECT WAREHOUSE</label>
              <select
                value={config.warehouse}
                onChange={(e) => setConfig({ ...config, warehouse: e.target.value })}
                className={inputCls}
              >
                <option value="">— Select Warehouse —</option>
                <option value="wh1">Warehouse A – Raw Materials</option>
                <option value="wh2">Warehouse B – Finished Goods</option>
              </select>
            </div>
            <div>
              <label className={labelCls}>CHARGES STRUCTURE</label>
              <select
                value={config.chargesStructure}
                onChange={(e) => setConfig({ ...config, chargesStructure: e.target.value })}
                className={inputCls}
              >
                <option value="">— Select Charges Structure —</option>
                <option value="std">Standard</option>
                <option value="premium">Premium</option>
              </select>
            </div>
          </div>
        </div>

        {/* ── Section 3: Select Returnable DC Reference ───────────── */}
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
          <div className="flex items-center justify-between px-6 py-4 border-b border-gray-100">
            <p className="text-sm font-bold tracking-tight text-gray-900 flex items-center gap-2">
              <span className="material-symbols-outlined text-base text-blue-600">swap_horiz</span>
              Select Returnable DC Reference
            </p>
            <button className="text-[11px] font-bold text-blue-600 border border-blue-200 rounded-lg px-3 py-1.5 hover:bg-blue-50 transition-colors flex items-center gap-1">
              <span className="material-symbols-outlined text-sm">refresh</span>
              REFRESH LIST
            </button>
          </div>
          <table className="w-full text-sm">
            <thead className="bg-gray-100">
              <tr>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">DC NUMBER</th>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">DC DATE</th>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">CUSTOMER / VENDOR</th>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">PURPOSE</th>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">EXPECTED RETURN</th>
                <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">STATUS</th>
              </tr>
            </thead>
            <tbody>
              {DC_REFERENCES.map((dc) => (
                <tr
                  key={dc.id}
                  onClick={() => setSelectedDcId(dc.id)}
                  className={`cursor-pointer border-t border-gray-100 transition-colors ${
                    selectedDcId === dc.id
                      ? 'bg-blue-50'
                      : 'hover:bg-blue-50/50'
                  }`}
                >
                  <td className="px-5 py-3 font-semibold text-blue-600">{dc.dcNumber}</td>
                  <td className="px-5 py-3 text-gray-700">{dc.dcDate}</td>
                  <td className="px-5 py-3 text-gray-700">{dc.customerVendor}</td>
                  <td className="px-5 py-3">
                    <span
                      className={`text-[11px] font-bold px-2 py-1 rounded-full ${
                        dc.purpose === 'JOB WORK'
                          ? 'bg-purple-100 text-purple-700'
                          : 'bg-orange-100 text-orange-700'
                      }`}
                    >
                      {dc.purpose}
                    </span>
                  </td>
                  <td className="px-5 py-3 text-gray-700">{dc.expectedReturn}</td>
                  <td className="px-5 py-3">
                    <span
                      className={`text-[11px] font-bold px-2 py-1 rounded-full ${
                        dc.status === 'PARTIALLY RETURNED'
                          ? 'bg-blue-100 text-blue-700'
                          : 'bg-yellow-100 text-yellow-700'
                      }`}
                    >
                      {dc.status}
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>

        {/* ── Section 4: Line Items ────────────────────────────────── */}
        <div className="bg-white border border-gray-200 rounded-xl overflow-hidden">
          <div className="px-6 py-4 border-b border-gray-100">
            <p className="text-sm font-bold tracking-tight text-gray-900 flex items-center gap-2">
              <span className="material-symbols-outlined text-base text-blue-600">inventory_2</span>
              Line Items – Material Receipt Details
            </p>
          </div>
          <div className="overflow-x-auto">
            <table className="w-full text-sm" style={{ minWidth: '1200px' }}>
              <thead className="bg-gray-100">
                <tr>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">MATERIAL DESCRIPTION</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">PUOM</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">DISPATCHED</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">RECEIVED</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">PENDING</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500 bg-blue-50">QTY TO RECV</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">QC STATUS</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">FREIGHT</th>
                  <th className="text-left px-5 py-3 text-[11px] font-bold text-gray-500">REMARKS</th>
                </tr>
              </thead>
              <tbody>
                {lineItems.map((item) => (
                  <tr key={item.id} className="border-t border-gray-100 hover:bg-gray-50/50">
                    <td className="px-5 py-3 font-medium text-gray-800">{item.materialDescription}</td>
                    <td className="px-5 py-3 text-gray-600">{item.puom}</td>
                    <td className="px-5 py-3 text-gray-600">{item.dispatched.toFixed(2)}</td>
                    <td className="px-5 py-3 text-gray-600">{item.received.toFixed(2)}</td>
                    <td className="px-5 py-3 text-gray-600">{item.pending.toFixed(2)}</td>
                    <td className="px-5 py-3 bg-blue-50/40">
                      <input
                        type="number"
                        value={item.qtyToRecv}
                        onChange={(e) => handleQtyChange(item.id, e.target.value)}
                        className="bg-white border border-blue-200 rounded-lg px-3 py-1.5 text-sm outline-none focus:ring-2 focus:ring-blue-200 w-24 text-right"
                      />
                    </td>
                    <td className="px-5 py-3">
                      <select
                        value={item.qcStatus}
                        onChange={(e) => handleQcChange(item.id, e.target.value)}
                        className={`border border-gray-200 rounded-lg px-3 py-1.5 text-sm outline-none focus:ring-2 focus:ring-blue-100 ${qcBg(item.qcStatus)}`}
                      >
                        <option value="Passed">Passed</option>
                        <option value="Pending">Pending</option>
                        <option value="Failed">Failed</option>
                      </select>
                    </td>
                    <td className="px-5 py-3 text-gray-700">{item.freight}</td>
                    <td className="px-5 py-3">
                      <input
                        type="text"
                        value={item.remarks}
                        onChange={(e) => handleRemarksChange(item.id, e.target.value)}
                        placeholder="Add note..."
                        className="bg-white border border-gray-200 rounded-lg px-3 py-1.5 text-sm outline-none focus:ring-2 focus:ring-blue-100 w-40"
                      />
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
          </div>
        </div>

        {/* ── Section 5: Logistics + Financial Preview ─────────────── */}
        <div className="grid grid-cols-3 gap-6">
          {/* Logistics — 2 cols */}
          <div className="col-span-2 bg-gray-50 border border-gray-200 p-8 rounded-xl">
            <p className="text-sm font-bold tracking-tight text-gray-900 mb-6 flex items-center gap-2">
              <span className="material-symbols-outlined text-base text-blue-600">local_shipping</span>
              Logistics and Inward Details
            </p>
            <div className="grid grid-cols-3 gap-5">
              <div>
                <label className={labelCls}>SUPPLIER CHALLAN NO</label>
                <input
                  value={logistics.supplierChallanNo}
                  onChange={(e) => setLogistics({ ...logistics, supplierChallanNo: e.target.value })}
                  placeholder="e.g. CH/24/1234"
                  className={inputCls}
                />
              </div>
              <div>
                <label className={labelCls}>CHALLAN DATE</label>
                <input
                  type="date"
                  value={logistics.challanDate}
                  onChange={(e) => setLogistics({ ...logistics, challanDate: e.target.value })}
                  className={inputCls}
                />
              </div>
              <div>
                <label className={labelCls}>GATE INWARD NO</label>
                <input
                  value={logistics.gateInwardNo}
                  onChange={(e) => setLogistics({ ...logistics, gateInwardNo: e.target.value })}
                  placeholder="e.g. GIN/24/0045"
                  className={inputCls}
                />
              </div>
              <div>
                <label className={labelCls}>TRANSPORTER</label>
                <input
                  value={logistics.transporter}
                  onChange={(e) => setLogistics({ ...logistics, transporter: e.target.value })}
                  placeholder="Transporter name"
                  className={inputCls}
                />
              </div>
              <div>
                <label className={labelCls}>VEHICLE NUMBER</label>
                <input
                  value={logistics.vehicleNumber}
                  onChange={(e) => setLogistics({ ...logistics, vehicleNumber: e.target.value })}
                  placeholder="e.g. MH12AB1234"
                  className={inputCls}
                />
              </div>
              <div>
                <label className={labelCls}>DRIVER CONTACT</label>
                <input
                  value={logistics.driverContact}
                  onChange={(e) => setLogistics({ ...logistics, driverContact: e.target.value })}
                  placeholder="+91 XXXXXXXXXX"
                  className={inputCls}
                />
              </div>
            </div>
          </div>

          {/* Right column: Financial Preview + Compliance */}
          <div className="col-span-1 flex flex-col gap-4">
            {/* Financial Preview */}
            <div className="bg-white border border-gray-200 p-8 rounded-xl flex-1">
              <p className="text-sm font-bold tracking-tight text-gray-900 mb-5 flex items-center gap-2">
                <span className="material-symbols-outlined text-base text-blue-600">account_balance_wallet</span>
                Financial Preview
              </p>
              <div className="space-y-3">
                <div className="flex justify-between text-sm text-gray-600">
                  <span>Total Freight</span>
                  <span className="font-medium text-gray-800">₹5,700.00</span>
                </div>
                <div className="flex justify-between text-sm text-gray-600">
                  <span>Insurance</span>
                  <span className="font-medium text-gray-800">₹250.00</span>
                </div>
                <div className="flex justify-between text-sm text-gray-600">
                  <span>Handling</span>
                  <span className="font-medium text-gray-800">₹180.00</span>
                </div>
                <div className="border-t border-dashed border-gray-300 my-3" />
                <div className="flex justify-between items-baseline">
                  <span className="text-sm font-bold text-gray-700">GRAND TOTAL</span>
                  <span className="text-xl font-bold text-blue-600">₹6,130.00</span>
                </div>
              </div>
            </div>

            {/* Compliance Check */}
            <div className="bg-blue-50/50 border border-blue-100 p-6 rounded-xl">
              <div className="flex items-start gap-3">
                <span className="material-symbols-outlined text-blue-600 text-xl mt-0.5">verified_user</span>
                <div>
                  <p className="text-[11px] font-bold text-blue-700 tracking-wide mb-1">COMPLIANCE CHECK</p>
                  <p className="text-xs text-blue-600">Material condition verified by QC Supervisor.</p>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* ── Section 6: Remarks + Attachments ─────────────────────── */}
        <div className="grid grid-cols-2 gap-6">
          {/* Remarks */}
          <div className="bg-white border border-gray-200 p-6 rounded-xl">
            <label className={`${labelCls} mb-2`}>ADDITIONAL REMARKS</label>
            <textarea
              value={remarks}
              onChange={(e) => setRemarks(e.target.value)}
              rows={5}
              placeholder="Add any additional notes or remarks for this GRN..."
              className="bg-white border border-gray-200 rounded-lg px-4 py-3 text-sm outline-none focus:ring-2 focus:ring-blue-100 w-full resize-none"
            />
          </div>

          {/* Attachments */}
          <div className="bg-white border border-gray-200 p-6 rounded-xl">
            <p className={`${labelCls} mb-3`}>ATTACHMENTS</p>
            {/* Drag-drop area */}
            <div className="border-2 border-dashed border-gray-200 rounded-xl p-6 flex flex-col items-center justify-center text-center gap-2 hover:border-blue-300 hover:bg-blue-50/30 transition-colors cursor-pointer mb-4">
              <span className="material-symbols-outlined text-3xl text-gray-300">upload_file</span>
              <p className="text-sm font-medium text-gray-500">Drag &amp; drop files here</p>
              <p className="text-xs text-gray-400">or click to browse — PDF, JPG, PNG up to 10 MB</p>
            </div>
            {/* Existing file */}
            <div className="flex items-center justify-between bg-gray-50 border border-gray-200 rounded-lg px-4 py-3">
              <div className="flex items-center gap-3">
                <span className="material-symbols-outlined text-red-500 text-xl">picture_as_pdf</span>
                <div>
                  <p className="text-sm font-medium text-gray-800">challan_copy.pdf</p>
                  <p className="text-xs text-gray-400">1.2 MB</p>
                </div>
              </div>
              <button className="text-gray-400 hover:text-red-500 transition-colors">
                <span className="material-symbols-outlined text-lg">close</span>
              </button>
            </div>
          </div>
        </div>
      </div>

      {/* ── Section 7: Sticky Footer ──────────────────────────────── */}
      <div className="sticky bottom-0 bg-white border-t border-gray-200 shadow-lg z-10">
        <div className="max-w-screen-xl mx-auto px-6 py-4 flex items-center justify-between">
          {/* Left */}
          <div className="flex items-center gap-4">
            <button className="flex items-center gap-2 text-sm font-medium text-gray-600 hover:text-gray-900 transition-colors">
              <span className="material-symbols-outlined text-base">print</span>
              Print Preview
            </button>
            <div className="w-px h-5 bg-gray-200" />
            <button
              onClick={() => router.back()}
              className="text-sm font-medium text-gray-500 hover:text-gray-800 transition-colors"
            >
              Cancel
            </button>
          </div>

          {/* Right */}
          <div className="flex items-center gap-3">
            <Button variant="outline" size="sm">
              Save as Draft
            </Button>
            <button className="flex items-center gap-2 bg-gradient-to-r from-blue-600 to-blue-500 hover:from-blue-700 hover:to-blue-600 text-white text-sm font-semibold px-5 py-2.5 rounded-lg shadow transition-all">
              <span className="material-symbols-outlined text-base">check_circle</span>
              Complete Receipt
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}
