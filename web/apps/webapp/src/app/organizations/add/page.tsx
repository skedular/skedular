'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddOrganizationPage = () => {
  const router = useRouter();
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
      {
        label: 'Organizations',
        href: '/organizations',
      },
    ],
    lastItemLabel: 'Add new organization',
  };

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddOrganization showCancel={true} onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddOrganizationPage);
