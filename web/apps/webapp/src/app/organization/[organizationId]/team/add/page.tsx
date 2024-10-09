'use client';

import { RootShell } from '@/components/rootShell';
import { AddTeam } from '@/components/team/addTeam';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const AddTeamPage = () => {
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

  return (
    <RootShell>
      <AddTeam organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(AddTeamPage);
