import type { AppBarBreadcrumbs } from 'components/appBar';
import { Notifications } from 'components/notification/notifications';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';

const NotificationsPage = () => {
  const breadcrumps: AppBarBreadcrumbs = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Notifications',
  };

  return (
    <RootShell appBarBreadcrumbs={breadcrumps}>
      <Notifications />
    </RootShell>
  );
};

export default memo(NotificationsPage);
