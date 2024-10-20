import type { AppBarBreadcrumbs } from 'components/appBar';
import { CustomerSettings } from 'components/customer/settings';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';

const Settings = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
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
