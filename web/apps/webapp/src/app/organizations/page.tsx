'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { Organizations } from '@/components/organization/organizations';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
import { memo } from 'react';

const OrganizationsPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        icon: <HomeIcon />,
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
