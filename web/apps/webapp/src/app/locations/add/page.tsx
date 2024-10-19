'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddLocationPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
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

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <AddLocation />
    </RootShell>
  );
};

export default memo(AddLocationPage);
