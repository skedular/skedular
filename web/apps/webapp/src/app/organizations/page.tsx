'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { Organizations } from '@/components/organization/organizations';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const OrganizationsPage = () => {
  const breadcrumps: appBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Organizations',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Organizations />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
