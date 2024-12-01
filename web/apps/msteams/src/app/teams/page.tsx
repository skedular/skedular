import { SwitchToModernUIContext } from '@repo/shared/libs/providers';
import { RootShell } from 'components/rootShell';
import { OldTeams, Teams } from 'components/team/teams';
import { memo, useContext } from 'react';
import { useParams } from 'react-router-dom';

const TeamsPage = () => {
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

  return (
    <RootShell>
      {!switchToModernUI && <OldTeams organizationId={finalOrganizationId} />}
      {switchToModernUI && <Teams organizationId={finalOrganizationId} />}
    </RootShell>
  );
};

export default memo(TeamsPage);
