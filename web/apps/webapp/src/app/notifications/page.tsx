'use client';

import { Notifications } from '@/components/notification/notifications';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const NotificationsPage = () => (
  <RootShell hideOrganizationSelector>
    <Notifications />
  </RootShell>
);

export default memo(NotificationsPage);
