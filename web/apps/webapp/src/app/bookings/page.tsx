'use client';

import { Bookings } from '@/components/booking/bookingsPage';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => {
  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <Bookings onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
