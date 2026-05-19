import { MarketplaceProductBookingDetails, MarketplaceProductBookingSignIn } from '@/components/marketplaceProductBooking';
import { OrganizationStoreFrontRootShell, UnauthenticatedOrganizationStoreFrontRootShell } from '@/components/rootShell';
import { useAuth } from '@workos-inc/authkit-nextjs/components';
import { memo } from 'react';

const RootPage = () => {
  const { user } = useAuth();

  if (user) {
    return (
      <OrganizationStoreFrontRootShell>
        <MarketplaceProductBookingDetails />
      </OrganizationStoreFrontRootShell>
    );
  }

  return (
    <UnauthenticatedOrganizationStoreFrontRootShell>
      <MarketplaceProductBookingSignIn />
    </UnauthenticatedOrganizationStoreFrontRootShell>
  );
};

export default memo(RootPage);
