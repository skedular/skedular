'use client';

import { getOrganizationBaseLink } from '@/components/links';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const RootPage = () => {
  const router = useRouter();

  const handleAdded = (id: string) => {
    router.push(getOrganizationBaseLink(id));
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell hideOrganizationSelector>
      <AddOrganization showCancel={true} onAdded={handleAdded} onCancel={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(RootPage);
