'use client';

import { Locations } from '@/components/location/locations';
import { OrganizationLocations } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { useParams } from 'next/navigation';
import { memo, useContext } from 'react';

const OrganizationsPage = () => {
  const switchToModernUI = useContext(SwitchToModernUIContext);
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

  return (
    <RootShell>
      {!switchToModernUI && <OrganizationLocations onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />}
      {switchToModernUI && <Locations organizationId={finalOrganizationId} />}
    </RootShell>
  );
};

export default memo(OrganizationsPage);
