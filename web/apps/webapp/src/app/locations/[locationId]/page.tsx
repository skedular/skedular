'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { getLocationBaseLink } from '@/components/location';
import { Location } from '@/components/location/locationPage';
import { RootShell } from '@/components/rootShell';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const LocationPage = () => {
  const { locationId } = useParams();
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
        label: 'Locations',
        href: '/locations',
      },
    ],
    lastItemLabel: getLocationBaseLink(finalLocationId),
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Location organizationId="" locationId={finalLocationId} />
    </RootShell>
  );
};

export default memo(LocationPage);
