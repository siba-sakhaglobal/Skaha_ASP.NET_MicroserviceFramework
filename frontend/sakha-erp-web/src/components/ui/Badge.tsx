import { clsx } from 'clsx';
import { STATUS_COLORS } from '@/lib/constants';

interface BadgeProps {
  status: string;
  className?: string;
}

export function Badge({ status, className }: BadgeProps) {
  const colors = STATUS_COLORS[status] || 'bg-gray-100 text-gray-700';
  return (
    <span className={clsx('inline-flex items-center rounded-full px-2.5 py-0.5 text-xs font-medium', colors, className)}>
      {status}
    </span>
  );
}
