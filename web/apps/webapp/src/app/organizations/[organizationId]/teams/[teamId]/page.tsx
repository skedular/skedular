'use client';

import { OrganizationTeam } from '@/components/organization/organizationTeam';
import { RootShell } from '@/components/rootShell';
import { Team } from '@/components/team/teamPage';
import { Breadcrumbs } from '@mui/material';
import { BodyIconTypography } from '@repo/shared/components/commons';
import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { useParams } from 'next/navigation';
import { memo, useContext } from 'react';

const TeamPage = () => {
  const switchToModernUI = useContext(SwitchToModernUIContext);
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

  if (switchToModernUI) {
    const breadcrumbs = (
      <Breadcrumbs>
        <BodyIconTypography label="Team Settings" />
      </Breadcrumbs>
    );

    <RootShell collapsed hideOrganizationSelector hideWelcomeMessage showBreadcrumps breadcrumbs={breadcrumbs}>
      <OrganizationTeam organizationId={finalOrganizationId} teamId={finalTeamId} />
    </RootShell>;
  }

  return (
    <RootShell>
      <Team organizationId={finalOrganizationId} teamId={finalTeamId} />
    </RootShell>
  );
};

export default memo(TeamPage);
