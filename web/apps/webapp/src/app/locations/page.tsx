'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { Locations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const LocationsPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Locations',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Locations />
    </RootShell>
  );
};

export default memo(LocationsPage);
