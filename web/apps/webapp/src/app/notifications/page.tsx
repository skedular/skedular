'use client';

import { Notifications } from '@/components/notification/notifications';
import { RootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => (
  <RootShell hideOrganizationSelector>
    <Notifications />
  </RootShell>
);

export default memo(RootPage);
