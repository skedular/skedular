'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { getLocationBaseLink } from '@/components/location/location-link';
import { Location } from '@/components/location/locationPage';
import { getOrganizationBaseLink, getOrganizationLocationsBaseLink } from '@/components/organization/organization-link';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const LocationPage = () => {
  const { organizationId, locationId } = useParams();
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

  let finalLocationId = '';

  if (typeof locationId === 'string') {
    finalLocationId = locationId;
  } else if (Array.isArray(locationId)) {
    if (typeof locationId[0] === 'undefined') {
      throw new Error('locationId is required');
    }

    finalLocationId = locationId[0];
  } else {
    throw new Error('locationId is required');
  }

  const breadcrumps: appBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
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
    lastItemLabel: getLocationBaseLink(finalLocationId, finalOrganizationId),
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Location organizationId={finalOrganizationId} locationId={finalLocationId} />
    </RootShell>
  );
};

export default memo(LocationPage);
