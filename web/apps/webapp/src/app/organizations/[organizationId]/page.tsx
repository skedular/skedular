'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { getOrganizationBaseLink } from '@/components/organization';
import { Organization } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const OrganizationPage = () => {
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
    lastItemLabel: getOrganizationBaseLink(finalOrganizationId),
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Organization organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationPage);
