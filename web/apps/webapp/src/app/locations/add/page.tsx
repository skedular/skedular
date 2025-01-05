'use client';

import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddLocationPage = () => {
  const router = useRouter();

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <AddLocation onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddLocationPage);
