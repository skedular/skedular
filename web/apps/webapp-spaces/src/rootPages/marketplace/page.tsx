import { GuestStoreFront } from '@/components/organizationStoreFrontGuest';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <GuestStoreFront />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <GuestStoreFront />
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
