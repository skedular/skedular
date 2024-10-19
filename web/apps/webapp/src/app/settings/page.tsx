'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { CustomerSettings } from '@/components/customer/settings';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const Settings = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Settings',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <CustomerSettings />
    </RootShell>
  );
};

export default memo(Settings);
