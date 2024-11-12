'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
import { useRouter } from 'next/navigation';
import { memo } from 'react';

const AddLocationPage = () => {
  const router = useRouter();
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
      {
        label: 'Locations',
        href: '/locations',
      },
    ],
    lastItemLabel: 'Add new location',
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
      <AddLocation onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddLocationPage);
