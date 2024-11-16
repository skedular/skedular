import { HomeIcon } from '@repo/shared/components/icons';
import type { AppBarBreadcrumbs } from 'components/appBar';
import { CustomerSettings } from 'components/customer/settings';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';
import { useParams } from 'react-router-dom';

const Settings = () => {
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
    ],
    lastItemLabel: 'Settings',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <CustomerSettings />
    </RootShell>
  );
};

export default memo(Settings);
