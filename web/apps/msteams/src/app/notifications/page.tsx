import type { AppBarBreadcrumb } from 'components/appBar';
import { Notifications } from 'components/notification/notifications';
import { RootShell } from 'components/rootShell';
import { memo } from 'react';

const NotificationsPage = () => {
  const breadcrumps: AppBarBreadcrumb = {
    items: [
      {
        label: 'Home',
        href: '/',
      },
    ],
    lastItemLabel: 'Notifications',
  };

  return (
    <RootShell appBarBreadcrumb={breadcrumps}>
      <Notifications />
    </RootShell>
  );
};

export default memo(NotificationsPage);
