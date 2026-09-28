import { clsx } from 'clsx';
import { STATUS_COLORS } from '@/lib/constants';

export function StatusBadge({ status }: { status: string }) {
  const colors = STATUS_COLORS[status] || 'bg-gray-100 text-gray-700';
  const dotColor = status === 'Active' || status === 'Completed' || status === 'Accepted' ? 'bg-green-500'
    : status === 'Cancelled' ? 'bg-red-500'
    : status === 'Draft' ? 'bg-gray-400'
    : 'bg-yellow-500';

  return (
    <span className={clsx('inline-flex items-center gap-1.5 rounded-full px-2.5 py-0.5 text-xs font-medium', colors)}>
      <span className={clsx('h-1.5 w-1.5 rounded-full', dotColor)} />
      {status}
    </span>
  );
}
