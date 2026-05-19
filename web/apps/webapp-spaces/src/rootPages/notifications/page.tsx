import { Notifications } from '@/components/notification/notifications';
import { NoOrganizationRootShell } from '@/components/rootShell';
import { memo } from 'react';

const RootPage = () => (
  <NoOrganizationRootShell hideOrganizationSelector>
    <Notifications />
  </NoOrganizationRootShell>
);

export default memo(RootPage);
