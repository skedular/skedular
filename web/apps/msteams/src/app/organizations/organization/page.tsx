import { HomeIcon } from '@repo/shared/components/icons';
import type { AppBarBreadcrumbs } from 'components/appBar';
import { Dashboard } from 'components/customer/dashboard';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const OrganizationPage = () => {
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
    lastItemIcon: <HomeIcon />,
    lastItemLabel: 'Home',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Dashboard organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(OrganizationPage);
