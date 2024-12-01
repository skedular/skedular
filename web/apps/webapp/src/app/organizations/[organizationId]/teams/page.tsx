'use client';

import { OrganizationTeams } from '@/components/organization/organizationPage';
import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
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
      {!switchToModernUI && <OrganizationTeams onReloadRequired={handleReloadRequired} organizationId={finalOrganizationId} />}
      {switchToModernUI && <Teams organizationId={finalOrganizationId} />}
    </RootShell>
  );
};

export default memo(OrganizationsPage);
