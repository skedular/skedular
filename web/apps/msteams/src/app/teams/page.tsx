import { OrganizationTeams } from 'components/organization/organizationTeams';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const TeamsPage = () => {
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
      <OrganizationTeams organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(TeamsPage);
