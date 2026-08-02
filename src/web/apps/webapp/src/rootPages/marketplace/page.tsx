import { GuestStoreFront } from '@/components/organizationStoreFrontGuest';
import { Loading } from '@/components/loading';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user, loading } = useAuth();
  if (loading) return <Loading />;

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
