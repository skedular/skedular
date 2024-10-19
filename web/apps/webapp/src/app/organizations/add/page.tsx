'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddOrganizationPage = () => {
  const breadcrumps: appBarBreadcrumbs = {
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
