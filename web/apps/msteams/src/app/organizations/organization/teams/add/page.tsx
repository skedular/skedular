import type { AppBarBreadcrumbs } from 'components/appBar';
import { RootShell } from 'components/rootShell';
import { AddTeam } from 'components/team/addTeam';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

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
    lastItemLabel: 'Add new team',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddTeam organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(AddTeamPage);
