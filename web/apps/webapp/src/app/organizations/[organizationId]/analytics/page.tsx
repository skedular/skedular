'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { getOrganizationBaseLink } from '@/components/organization/organization-link';
import { OrganizationAnalytics } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const OrganizationsPage = () => {
  const { organizationId } = useParams();
  let finalOrganizationId = '';

  if (typeof organizationId === 'string') {
    finalOrganizationId = organizationId;
  } else if (Array.isArray(organizationId)) {
    if (typeof organizationId[0] === 'undefined') {
      throw new Error('organizationId is required');
    }

    finalOrganizationId = organizationId[0];
  } else {
    throw new Error('organizationId is required');
  }

  const handleReloadRequired = () => {};

  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
      {
        label: 'Organizations',
        href: getOrganizationBaseLink(finalOrganizationId),
      },
    ],
    lastItemLabel: 'Analytics',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <OrganizationAnalytics onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
