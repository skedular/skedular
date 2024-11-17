import { HomeIcon } from '@repo/shared/components/icons';
import type { AppBarBreadcrumbs } from 'components/appBar';
import { CustomerSettings } from 'components/customer/settings';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';

const Settings = () => {
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
