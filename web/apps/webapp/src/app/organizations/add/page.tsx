'use client';

import type { AppBarBreadcrumb } from '@/components/appBar';
import { AddOrganization } from '@/components/organization/addOrganization';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const AddOrganizationPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
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
    <RootShell appBarBreadcrumb={breadcrumps}>
      <AddOrganization />
    </RootShell>
  );
};

export default memo(AddOrganizationPage);
