import { GuestStoreFront } from '@/components/organizationStoreFrontGuest';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user, loading } = useAuth();

  if (loading) {
    return null;
  }

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <>User is signed in</>
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
