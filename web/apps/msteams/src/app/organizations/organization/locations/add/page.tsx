import { HomeIcon } from '@repo/shared/components/icons';
import type { AppBarBreadcrumbs } from 'components/appBar';
import { AddLocation } from 'components/location/addLocation';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const AddLocationPage = () => {
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
        icon: <HomeIcon />,
        label: 'Home',
        href: '/',
      },
      {
        label: 'Locations',
        href: '/locations',
      },
    ],
    lastItemLabel: 'Add new location',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <AddLocation organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(AddLocationPage);
