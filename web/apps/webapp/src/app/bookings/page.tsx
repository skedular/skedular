'use client';

import { OldBookings } from '@/components/booking';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => {
  const handleReloadRequired = () => {};

  return (
    <RootShell>
      <OldBookings onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
