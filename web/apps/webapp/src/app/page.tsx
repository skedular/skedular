'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { Dashboard } from '@/components/customer/dashboard';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Home = () => {
  const breadcrumps: appBarBreadcrumbs = {
    lastItemLabel: 'Home',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Dashboard />
    </RootShell>
  );
};

export default memo(Home);
