'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { Locations } from '@/components/location/locations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const LocationsPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Locations',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Locations />
    </RootShell>
  );
};

export default memo(LocationsPage);
