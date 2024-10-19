'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { Locations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const LocationsPage = () => {
  const breadcrumps: appBarBreadcrumbs = {
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
