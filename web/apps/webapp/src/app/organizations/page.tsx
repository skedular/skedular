'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { Organizations } from '@/components/organization/organizations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Organizations',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Organizations />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
