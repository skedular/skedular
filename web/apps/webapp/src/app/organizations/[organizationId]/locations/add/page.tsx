'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { AddLocation } from '@/components/location/addLocation';
import { getOrganizationBaseLink, getOrganizationLocationsBaseLink } from '@/components/organization';
import { RootShell } from '@/components/rootShell';
import { HomeIcon } from '@repo/shared/components/icons';
import { useParams, useRouter } from 'next/navigation';
import { memo } from 'react';

const AddLocationPage = () => {
  const router = useRouter();
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
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
      {
        label: 'Organizations',
        href: '/organizations',
      },
      {
        label: '',
        href: getOrganizationBaseLink(finalOrganizationId),
      },
      {
        label: 'Locations',
        href: getOrganizationLocationsBaseLink(finalOrganizationId),
      },
    ],
    lastItemLabel: 'Add new location',
  };

  const handleAdded = () => {
    router.back();
  };

  const handleCancelled = () => {
    router.back();
  };

  const handleReloadRequired = () => {};

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddLocation organizationId={finalOrganizationId} onAdded={handleAdded} onCancelled={handleCancelled} onReloadRequired={handleReloadRequired} />
    </RootShell>
  );
};

export default memo(AddLocationPage);
