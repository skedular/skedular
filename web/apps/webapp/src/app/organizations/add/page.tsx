'use client';

import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddOrganizationPage = () => {
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
      <AddOrganization showCancel={true} onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddOrganizationPage);
