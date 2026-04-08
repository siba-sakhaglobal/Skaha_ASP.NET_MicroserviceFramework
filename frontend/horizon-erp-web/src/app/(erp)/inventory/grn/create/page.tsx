'use client';

import { useSearchParams } from 'next/navigation';
import { Suspense } from 'react';
import { LoadingOverlay } from '@/components/feedback/LoadingOverlay';
import dynamic from 'next/dynamic';

const GrnWithPOForm = dynamic(() => import('@/modules/inventory/components/GrnWithPOForm'), { loading: () => <LoadingOverlay /> });
const GrnWithoutPOForm = dynamic(() => import('@/modules/inventory/components/GrnWithoutPOForm'), { loading: () => <LoadingOverlay /> });
const GrnCustomerSuppliedForm = dynamic(() => import('@/modules/inventory/components/GrnCustomerSuppliedForm'), { loading: () => <LoadingOverlay /> });
const GrnReturnableDCForm = dynamic(() => import('@/modules/inventory/components/GrnReturnableDCForm'), { loading: () => <LoadingOverlay /> });

function CreateGrnContent() {
  const searchParams = useSearchParams();
  const type = searchParams.get('type') || 'with-po';

  switch (type) {
    case 'with-po': return <GrnWithPOForm />;
    case 'without-po': return <GrnWithoutPOForm />;
    case 'customer-supplied': return <GrnCustomerSuppliedForm />;
    case 'returnable-dc': return <GrnReturnableDCForm />;
    default: return <GrnWithPOForm />;
  }
}

export default function CreateGrnPage() {
  return (
    <Suspense fallback={<LoadingOverlay />}>
      <CreateGrnContent />
    </Suspense>
  );
}
