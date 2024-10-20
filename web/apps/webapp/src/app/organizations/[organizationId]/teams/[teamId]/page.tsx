'use client';

import type { appBarBreadcrumbs } from '@/components/appBar';
import { getOrganizationBaseLink, getOrganizationTeamsBaseLink } from '@/components/organization';
import { RootShell } from '@/components/rootShell';
import { getTeamBaseLink } from '@/components/team';
import { Team } from '@/components/team/teamPage';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const TeamPage = () => {
  const { organizationId, teamId } = useParams();
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

  let finalTeamId = '';

  if (typeof teamId === 'string') {
    finalTeamId = teamId;
  } else if (Array.isArray(teamId)) {
    if (typeof teamId[0] === 'undefined') {
      throw new Error('teamId is required');
    }

    finalTeamId = teamId[0];
  } else {
    throw new Error('teamId is required');
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
        label: 'Teams',
        href: getOrganizationTeamsBaseLink(finalOrganizationId),
      },
    ],
    lastItemLabel: getTeamBaseLink(finalTeamId, finalOrganizationId),
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Team organizationId={finalOrganizationId} teamId={finalTeamId} />
    </RootShell>
  );
};

export default memo(TeamPage);
