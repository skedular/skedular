import { MarketplaceProductSubscriptionDetails, MarketplaceProductSubscriptionDetailsSignIn } from '@/components/marketplaceProductSubscription';
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
        <MarketplaceProductSubscriptionDetails />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <MarketplaceProductSubscriptionDetailsSignIn />
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
