'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { Dashboard } from '@/components/customer/dashboard';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

const Home = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    lastItemIcon: <HomeIcon />,
    lastItemLabel: 'Home',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Dashboard />
    </RootShell>
  );
};

export default memo(Home);
