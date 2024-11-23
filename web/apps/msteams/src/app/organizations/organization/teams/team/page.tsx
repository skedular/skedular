import { RootShell } from 'components/rootShell';
import { Team } from 'components/team/teamPage';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

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

  return (
    <RootShell>
      <Team organizationId={finalOrganizationId} teamId={finalTeamId} />
    </RootShell>
  );
};

export default memo(TeamPage);
