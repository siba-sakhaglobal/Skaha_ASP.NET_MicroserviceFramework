import { Sidebar } from '@/components/layout/Sidebar';
import { Header } from '@/components/layout/Header';

export default function ErpLayout({ children }: { children: React.ReactNode }) {
  return (
    <div className="min-h-screen bg-[var(--color-bg-secondary)]">
      <Header />
      <Sidebar />
      <main className="ml-[var(--sidebar-width)] pt-[var(--header-height)] p-6">
        {children}
      </main>
    </div>
  );
}
