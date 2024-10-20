'use client';

import type { AppBarBreadcrumbs } from '@/components/appBar';
import { RootShell } from '@/components/rootShell';
import { getTeamBaseLink } from '@/components/team';
import { Team } from '@/components/team/teamPage';
import { useParams } from 'next/navigation';
import { memo } from 'react';

const TeamPage = () => {
  const { teamId } = useParams();
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

  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
      {
        label: 'Teams',
        href: '/teams',
      },
    ],
    lastItemLabel: getTeamBaseLink(finalTeamId),
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Team organizationId="" teamId={finalTeamId} />
    </RootShell>
  );
};

export default memo(TeamPage);
