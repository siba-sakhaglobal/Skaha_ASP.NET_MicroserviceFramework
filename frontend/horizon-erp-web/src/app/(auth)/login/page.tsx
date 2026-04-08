'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { Button } from '@/components/ui/Button';
import { Input } from '@/components/ui/Input';
import { useAuth } from '@/contexts/AuthContext';
import { useTenant } from '@/contexts/TenantContext';

export default function LoginPage() {
  const router = useRouter();
  const { login } = useAuth();
  const { setTenant } = useTenant();
  const [isLoading, setIsLoading] = useState(false);

  const handleDevLogin = () => {
    setIsLoading(true);
    // Dev mode: skip auth, set mock user
    login('dev-token', 'dev-refresh', {
      userId: 'dev-001', email: 'dev@sourcepro.co.in', name: 'Dev User',
      role: 'Admin', permissions: ['inventory.*', 'purchase.*', 'sales.*'], assignedSites: ['site-1']
    });
    setTenant({
      tenantId: 'aaaaaaaa-bbbb-cccc-dddd-eeeeeeeeeeee', tenantName: 'SourcePro Dev',
      enabledModules: ['inventory', 'purchase', 'sales', 'items', 'settings'],
      theme: { primaryColor: '#1a56db', logo: '', template: 'modern' },
      features: { aiChat: false }
    });
    router.push('/inventory');
  };

  return (
    <div className="min-h-screen flex items-center justify-center bg-gray-50">
      <div className="bg-white rounded-xl shadow-lg p-8 w-full max-w-md">
        <h1 className="text-2xl font-bold text-center text-[var(--color-primary)] mb-2">Horizon Premier</h1>
        <p className="text-sm text-gray-500 text-center mb-8">Enterprise Resource Planning</p>
        <div className="space-y-4">
          <Input label="Email" type="email" placeholder="user@company.com" />
          <Input label="Password" type="password" placeholder="Enter password" />
          <Button className="w-full" isLoading={isLoading} onClick={handleDevLogin}>Sign In</Button>
        </div>
        <div className="mt-4 pt-4 border-t border-gray-100">
          <Button variant="ghost" className="w-full text-xs" onClick={handleDevLogin}>
            Dev Login (Skip Auth)
          </Button>
        </div>
      </div>
    </div>
  );
}
