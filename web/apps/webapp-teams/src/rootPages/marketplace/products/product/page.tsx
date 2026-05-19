import { MarketplaceProductDetail } from '@/components/marketplaceProductGuest';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <MarketplaceProductDetail />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <MarketplaceProductDetail />
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
