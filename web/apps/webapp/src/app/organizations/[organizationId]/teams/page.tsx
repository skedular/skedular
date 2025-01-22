'use client';

import { RootShell } from '@/components/rootShell';
import { Teams } from '@/components/team/teams';
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

  return (
    <RootShell>
      <Teams organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationsPage);
