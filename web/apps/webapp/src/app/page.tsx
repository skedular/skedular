'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { Dashboard } from '@/components/customer/dashboard';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Home = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    lastItemLabel: 'Home',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Dashboard />
    </RootShell>
  );
};

export default memo(Home);
