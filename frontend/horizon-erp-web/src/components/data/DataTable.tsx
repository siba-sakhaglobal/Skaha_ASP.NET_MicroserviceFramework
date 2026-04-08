'use client';

import { useState, type ReactNode } from 'react';
import { clsx } from 'clsx';

export interface Column<T> {
  key: string;
  header: string;
  render?: (row: T) => ReactNode;
  sortable?: boolean;
  width?: string;
}

interface DataTableProps<T> {
  columns: Column<T>[];
  data: T[];
  isLoading?: boolean;
  onRowClick?: (row: T) => void;
  emptyMessage?: string;
  keyExtractor?: (row: T) => string;
}

export function DataTable<T extends Record<string, any>>({
  columns, data, isLoading, onRowClick, emptyMessage = 'No data found', keyExtractor
}: DataTableProps<T>) {
  if (isLoading) {
    return (
      <div className="bg-white rounded-xl border border-gray-200 p-12 text-center">
        <div className="animate-spin h-8 w-8 border-4 border-gray-200 border-t-[var(--color-primary)] rounded-full mx-auto" />
        <p className="mt-3 text-sm text-gray-500">Loading...</p>
      </div>
    );
  }

  return (
    <div className="bg-white rounded-xl border border-gray-200 overflow-hidden">
      <div className="overflow-x-auto">
        <table className="w-full text-sm">
          <thead>
            <tr className="bg-gray-50 border-b border-gray-200">
              {columns.map(col => (
                <th key={col.key} className="text-left px-4 py-3 font-medium text-gray-600" style={{ width: col.width }}>
                  {col.header}
                </th>
              ))}
            </tr>
          </thead>
          <tbody>
            {data.length === 0 ? (
              <tr><td colSpan={columns.length} className="px-4 py-12 text-center text-gray-400">{emptyMessage}</td></tr>
            ) : (
              data.map((row, idx) => (
                <tr
                  key={keyExtractor?.(row) ?? idx}
                  className={clsx('border-b border-gray-100 transition-colors', onRowClick && 'cursor-pointer hover:bg-gray-50')}
                  onClick={() => onRowClick?.(row)}
                >
                  {columns.map(col => (
                    <td key={col.key} className="px-4 py-3 text-gray-700">
                      {col.render ? col.render(row) : row[col.key]}
                    </td>
                  ))}
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}
