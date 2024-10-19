import type { AppBarBreadcrumb } from 'components/appBar';
import { Locations } from 'components/location/locations';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const LocationsPage = () => {
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

  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Locations',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Locations organizationId={finalOrganizationId} />
    </RootShell>
  );
};

export default memo(LocationsPage);
