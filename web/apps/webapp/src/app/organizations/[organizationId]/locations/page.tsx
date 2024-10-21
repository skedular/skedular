'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { getOrganizationBaseLink } from '@/components/organization/organization-link';
import { OrganizationLocations } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
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
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
      {
        label: 'Organizations',
        href: getOrganizationBaseLink(finalOrganizationId),
      },
    ],
    lastItemLabel: 'Locations',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <OrganizationLocations onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
