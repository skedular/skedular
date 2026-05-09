import { MarketplaceProductSubscribe, MarketplaceProductSubscribeSignIn } from '@/components/marketplaceProductSubscription';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <MarketplaceProductSubscribe />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <MarketplaceProductSubscribeSignIn />
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
