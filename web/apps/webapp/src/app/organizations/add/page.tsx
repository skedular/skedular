'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddOrganizationPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
      {
        label: 'Organizations',
        href: '/organizations',
      },
    ],
    lastItemLabel: 'Add new organization',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddOrganization />
    </RootShell>
  );
};

export default memo(AddOrganizationPage);
