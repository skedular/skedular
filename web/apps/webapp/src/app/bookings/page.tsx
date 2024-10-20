'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { Bookings } from '@/components/booking/bookingsPage';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => {
  const handleReloadRequired = () => {};

  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Bookings',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Bookings onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
