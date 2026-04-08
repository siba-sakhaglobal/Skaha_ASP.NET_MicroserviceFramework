import { type ReactNode } from 'react';

interface PageHeaderProps {
  title: string;
  subtitle?: string;
  actions?: ReactNode;
  breadcrumbs?: { label: string; href?: string }[];
}

export function PageHeader({ title, subtitle, actions, breadcrumbs }: PageHeaderProps) {
  return (
    <div className="mb-6">
      {breadcrumbs && (
        <nav className="text-sm text-gray-500 mb-2">
          {breadcrumbs.map((b, i) => (
            <span key={i}>
              {i > 0 && <span className="mx-1">/</span>}
              {b.href ? <a href={b.href} className="hover:text-[var(--color-primary)]">{b.label}</a> : <span>{b.label}</span>}
            </span>
          ))}
        </nav>
      )}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-bold text-gray-900">{title}</h1>
          {subtitle && <p className="text-sm text-gray-500 mt-1">{subtitle}</p>}
        </div>
        {actions && <div className="flex items-center gap-2">{actions}</div>}
      </div>
    </div>
  );
}
