'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { AddLocation } from '@/components/location/addLocation';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddLocationPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
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
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddLocation />
    </RootShell>
  );
};

export default memo(AddLocationPage);
