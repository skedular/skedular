'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { Dashboard } from '@/components/customer/dashboard';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Home = () => {
  const breadcrumps: AppBarBreadcrumb = {
    lastItemLabel: 'Home',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Dashboard />
    </RootShell>
  );
};

export default memo(Home);
